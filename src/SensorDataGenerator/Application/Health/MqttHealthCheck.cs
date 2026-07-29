using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SensorDataGenerator.Application.Output;
using SensorDataGenerator.Configuration;

namespace SensorDataGenerator.Application.Health;

public sealed class MqttHealthCheck : IHealthCheck
{
    private const int StartupGraceSeconds = 30;

    private readonly MqttSettings _settings;
    private readonly MqttOutputSink _sink;
    private static readonly DateTime AppStartTime = DateTime.UtcNow;

    public MqttHealthCheck(IOptions<MqttSettings> options, MqttOutputSink sink)
    {
        _settings = options.Value;
        _sink = sink;

    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_settings.Enabled)
        {
            return Task.FromResult(HealthCheckResult.Healthy("MQTT is disabled"));
        }

        var data = new Dictionary<string, object>
        {
            ["IsConnected"] = _sink.IsConnected,
            ["LastSuccessfulWrite"] = _sink.LastSuccessfulWrite?.ToString("O") ?? "never",
            ["LastError"] = _sink.LastError?.ToString("O") ?? "never",
            ["ConsecutiveErrorCount"] = _sink.ConsecutiveErrorCount,
            ["BufferCount"] = _sink.BufferCount,
            ["BufferCapacity"] = _sink.BufferCapacityValue,
            ["MessagesDropped"] = _sink.MessagesDropped
        };

        var uptime = DateTime.UtcNow - AppStartTime;
        var inGracePeriod = uptime.TotalSeconds < StartupGraceSeconds;

        // Messages are being dropped — buffer is saturated, broker unreachable
        if (_sink.MessagesDropped > 0)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "MQTT buffer full, messages being dropped", null, data));
        }

        // Buffer is filling up — warn if above 80%
        if (_sink.BufferCount > _sink.BufferCapacityValue * 0.8)
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                $"MQTT buffer {_sink.BufferCount}/{_sink.BufferCapacityValue} — broker may be unreachable", null, data));
        }

        // Not connected
        if (!_sink.IsConnected)
        {
            if (inGracePeriod)
            {
                return Task.FromResult(HealthCheckResult.Healthy(
                    "MQTT not yet connected (startup grace period)", data));
            }

            // Connected before but now disconnected with data in buffer — likely a broker issue
            if (_sink.BufferCount > 0)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    "MQTT disconnected, buffering messages", null, data));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy(
                "MQTT disconnected", null, data));
        }

        // Connected but with consecutive errors
        if (_sink.ConsecutiveErrorCount > 0)
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                $"MQTT connected but {_sink.ConsecutiveErrorCount} consecutive publish failure(s)",
                data: data));
        }

        // Connected and never published yet (within grace)
        if (_sink.LastSuccessfulWrite is null && inGracePeriod)
        {
            return Task.FromResult(HealthCheckResult.Healthy(
                "MQTT connected, awaiting first publish (startup grace period)", data));
        }

        return Task.FromResult(HealthCheckResult.Healthy("MQTT healthy", data));
    }
}
