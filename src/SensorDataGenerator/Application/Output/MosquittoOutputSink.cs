using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;
using SensorDataGenerator.Configuration;

namespace SensorDataGenerator.Application.Output;

public sealed class MqttOutputSink : IOutputSink, IAsyncDisposable
{
    private const int BufferCapacity = 10000;

    private readonly MqttSettings _settings;
    private readonly ILogger<MqttOutputSink> _logger;
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _options;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly CancellationTokenSource _cts = new();
    private readonly Channel<MqttApplicationMessage> _channel;
    private readonly Task _drainTask;
    private int _messagesDropped;

    public bool IsConnected => _client.IsConnected;
    public DateTime? LastSuccessfulWrite { get; private set; }
    public DateTime? LastError { get; private set; }
    public int ConsecutiveErrorCount { get; private set; }
    public int BufferCount => _channel.Reader.Count;
    public int BufferCapacityValue => BufferCapacity;
    public int MessagesDropped => Volatile.Read(ref _messagesDropped);

    public MqttOutputSink(IOptions<MqttSettings> options, ILogger<MqttOutputSink> logger)
    {
        _settings = options.Value;
        _logger = logger;

        var factory = new MqttClientFactory();
        _client = factory.CreateMqttClient();
        _options = BuildOptions();

        _client.ConnectedAsync += e =>
        {
            _logger.LogInformation("MQTT connected to {Host}:{Port}", _settings.Host, _settings.Port);
            return Task.CompletedTask;
        };

        _client.DisconnectedAsync += e =>
        {
            _logger.LogWarning("MQTT disconnected. Reason: {Reason}", e.Reason);
            return Task.CompletedTask;
        };

        var channelOptions = new BoundedChannelOptions(BufferCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        };
        _channel = Channel.CreateBounded<MqttApplicationMessage>(channelOptions);

        _drainTask = Task.Run(() => DrainAsync(_cts.Token));
    }

    public Task WriteAsync(
        string formattedMessage,
        string sensorId,
        string fileExtension,
        CancellationToken cancellationToken)
    {
        var topic = $"{_settings.TopicPrefix}/{sensorId}";

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(formattedMessage))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        if (!_channel.Writer.TryWrite(message))
        {
            _logger.LogWarning("MQTT buffer full, dropping message for {Topic}", topic);
            Interlocked.Increment(ref _messagesDropped);
        }

        return Task.CompletedTask;
    }

    private async Task DrainAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _channel.Reader.ReadAsync(cancellationToken);

                var published = false;
                try
                {
                    await EnsureConnectedAsync(cancellationToken);

                    var result = await _client.PublishAsync(message, cancellationToken);

                    if (result.ReasonCode == MqttClientPublishReasonCode.Success)
                    {
                        _logger.LogDebug("Published to {Topic}", message.Topic);
                        published = true;
                    }
                    else if (result.ReasonCode == MqttClientPublishReasonCode.NoMatchingSubscribers)
                    {
                        _logger.LogDebug("Published to {Topic} (no subscribers)", message.Topic);
                        published = true;
                    }
                    else
                    {
                        _logger.LogWarning("Publish to {Topic} returned {ReasonCode}", message.Topic, result.ReasonCode);
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish to {Topic}", message.Topic);
                }

                if (published)
                {
                    LastSuccessfulWrite = DateTime.UtcNow;
                    ConsecutiveErrorCount = 0;
                }
                else
                {
                    LastError = DateTime.UtcNow;
                    ConsecutiveErrorCount++;

                    if (_client.IsConnected)
                    {
                        await _client.DisconnectAsync(
                            new MqttClientDisconnectOptionsBuilder()
                                .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                                .Build(),
                            cancellationToken);
                    }

                    await Task.Delay(
                        TimeSpan.FromSeconds(_settings.ReconnectDelaySeconds),
                        cancellationToken);
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_client.IsConnected)
            return;

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_client.IsConnected)
                return;

            _logger.LogInformation("Connecting to MQTT broker {Host}:{Port}...", _settings.Host, _settings.Port);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token);
            linkedCts.CancelAfter(TimeSpan.FromSeconds(10));

            var result = await _client.ConnectAsync(_options, linkedCts.Token);

            if (result.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new InvalidOperationException(
                    $"Failed to connect to MQTT broker. Result: {result.ResultCode}");
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private MqttClientOptions BuildOptions()
    {
        var builder = new MqttClientOptionsBuilder()
            .WithTcpServer(_settings.Host, _settings.Port)
            .WithClientId(_settings.ClientId)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(_settings.KeepAliveSeconds))
            .WithCleanSession()
            .WithTimeout(TimeSpan.FromSeconds(10));

        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            builder.WithCredentials(_settings.Username, _settings.Password);
        }

        if (_settings.UseTls)
        {
            builder.WithTlsOptions(o => o.UseTls());
        }

        return builder.Build();
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _channel.Writer.TryComplete();

        try { await _drainTask; } catch (OperationCanceledException) { }

        if (_client.IsConnected)
        {
            await _client.DisconnectAsync(new MqttClientDisconnectOptionsBuilder()
                .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                .Build());
        }

        _client.Dispose();
        _connectionLock.Dispose();
        _cts.Dispose();
    }
}
