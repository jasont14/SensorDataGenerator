using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SensorDataGenerator.Application.Messaging;
using SensorDataGenerator.Application.Output;
using SensorDataGenerator.Configuration;
using SensorDataGenerator.Domain.Models;
using Microsoft.Extensions.Logging;
using SensorDataGenerator.Application.Messaging.Dtos;

namespace SensorDataGenerator.Application.Services;

public class SensorSimulationService : BackgroundService
{
    private readonly SensorSettings _settings;
    private readonly IMessageFormatter _formatter;
    private readonly IEnumerable<IOutputSink> _sinks;
    private readonly Random _random = new();
    private readonly ILogger<SensorSimulationService> _logger;
    private readonly ISensorPayloadMapper _mapper;

    public SensorSimulationService(
        IOptions<SensorSettings> options,
        IMessageFormatter formatter,
        IEnumerable<IOutputSink> sinks,
        ILogger<SensorSimulationService> logger,
        ISensorPayloadMapper mapper)
    {
        _settings = options.Value;
        _formatter = formatter;
        _sinks = sinks;
        _logger = logger;
        _mapper = mapper;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting sensor simulation with {SensorCount} sensors", _settings.Sensors.Count);
        _logger.LogInformation("Console output enabled: {ConsoleEnabled}", _settings.Output.ConsoleEnabled);
        _logger.LogInformation("File output enabled: {FileEnabled}", _settings.Output.FileEnabled);
        _logger.LogInformation("Output format: {Format}", _settings.Output.Format);

        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var sensor in _settings.Sensors)
            {
                var message = CreateMessage(sensor);
                var formatted = _formatter.Format(message);

                foreach (var sink in _sinks)
                {
                    await sink.WriteAsync(formatted, sensor.Id, _formatter.FileExtension, cancellationToken);
                }

                _logger.LogDebug("Generated reading for {SensorId} ({SensorType})", sensor.Id, sensor.Type);
            }

            try
            {
                await Task.Delay(_settings.IntervalMilliseconds, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Cancellation requested. Stopping simulation");
                break;
            }
        }

        _logger.LogInformation("Sensor simulation stopped");
    }

    private SensorMessage CreateMessage(SensorInstanceSettings sensor)
    {
        var now = DateTimeOffset.UtcNow;

        object payload = sensor.Type.ToLowerInvariant() switch
        {
            "weather" => _mapper.Map(GenerateWeather(now)),
            "sound" => _mapper.Map(GenerateSound(now)),
            "electrical" => _mapper.Map(GenerateElectrical(now)),
            "ph" => _mapper.Map(GeneratePh(now)),
            "orp" => _mapper.Map(GenerateOrp(now)),
            "conductivity" => _mapper.Map(GenerateConductivity(now)),
            "chemicalconcentration" => _mapper.Map(GenerateChemicalConcentration(now)),
            "spectroscopic" => _mapper.Map(GenerateSpectroscopic(now)),
            "dissolvedoxygen" => _mapper.Map(GenerateDissolvedOxygen(now)),
            "toxicgas" => _mapper.Map(GenerateToxicGas(now)),
            "combustiblegas" => _mapper.Map(GenerateCombustibleGas(now)),
            "photoionization" => _mapper.Map(GeneratePhotoionization(now)),
            "level" => _mapper.Map(GenerateLevel(now)),
            "massflow" => _mapper.Map(GenerateMassFlow(now)),
            "loadcell" => _mapper.Map(GenerateLoadCell(now)),
            _ => throw new InvalidOperationException($"Unknown sensor type: {sensor.Type}")
        };

        return new SensorMessage
        {
            SensorId = sensor.Id,
            SensorType = sensor.Type,
            Timestamp = now,
            Payload = payload
        };
    }

    private WeatherReading GenerateWeather(DateTimeOffset timestamp)
    {
        var s = _settings.Weather;
        return new WeatherReading(
            timestamp,
            TemperatureCelsius: RandomDouble(s.MinTemp, s.MaxTemp),
            HumidityPercent: RandomDouble(s.MinHumidity, s.MaxHumidity),
            PressureHpa: s.BasePressure + RandomDouble(-5.0, 5.0)
        );
    }

    private SoundReading GenerateSound(DateTimeOffset timestamp)
    {
        var s = _settings.Sound;
        return new SoundReading(timestamp, RandomDouble(s.MinDb, s.MaxDb));
    }

    private ElectricalReading GenerateElectrical(DateTimeOffset timestamp)
    {
        var s = _settings.Electrical;
        var volts = s.NominalVoltage + RandomDouble(-s.VoltageVariance, s.VoltageVariance);
        var amps = RandomDouble(s.MinAmps, s.MaxAmps);
        return new ElectricalReading(timestamp, volts, amps);
    }

    private PhReading GeneratePh(DateTimeOffset timestamp)
    {
        var s = _settings.Ph;
        return new PhReading(timestamp, RandomDouble(s.MinPh, s.MaxPh));
    }

    private OrpReading GenerateOrp(DateTimeOffset timestamp)
    {
        var s = _settings.Orp;
        return new OrpReading(timestamp, RandomDouble(s.MinMv, s.MaxMv));
    }

    private ConductivityReading GenerateConductivity(DateTimeOffset timestamp)
    {
        var s = _settings.Conductivity;
        return new ConductivityReading(timestamp, RandomDouble(s.MinMsCm, s.MaxMsCm));
    }

    private ChemicalConcentrationReading GenerateChemicalConcentration(DateTimeOffset timestamp)
    {
        var s = _settings.ChemicalConcentration;
        return new ChemicalConcentrationReading(timestamp, RandomDouble(s.MinPercent, s.MaxPercent));
    }

    private SpectroscopicReading GenerateSpectroscopic(DateTimeOffset timestamp)
    {
        var s = _settings.Spectroscopic;
        var numPoints = s.NumPoints;
        var wavenumbers = new double[numPoints];
        var intensities = new double[numPoints];

        var step = (s.MaxWavenumber - s.MinWavenumber) / (double)(numPoints - 1);

        var peaks = new (double center, double amplitude, double width)[]
        {
            (1500, 0.7, 200),
            (2900, 0.9, 150),
            (1650, 0.5, 100),
            (3300, 0.6, 300)
        };

        for (int i = 0; i < numPoints; i++)
        {
            double wn = s.MinWavenumber + i * step;
            wavenumbers[i] = Math.Round(wn, 1);

            double intensity = _random.NextDouble() * 0.1;

            foreach (var (center, amplitude, width) in peaks)
            {
                double diff = wn - center;
                intensity += amplitude * Math.Exp(-(diff * diff) / (2 * width * width));
            }

            intensity += (_random.NextDouble() - 0.5) * 0.05;

            intensities[i] = Math.Clamp(intensity, s.MinIntensity, s.MaxIntensity);
        }

        return new SpectroscopicReading(timestamp, wavenumbers, intensities);
    }

    private DissolvedOxygenReading GenerateDissolvedOxygen(DateTimeOffset timestamp)
    {
        var s = _settings.DissolvedOxygen;
        return new DissolvedOxygenReading(timestamp, RandomDouble(s.MinPpm, s.MaxPpm));
    }

    private ToxicGasReading GenerateToxicGas(DateTimeOffset timestamp)
    {
        var s = _settings.ToxicGas;
        return new ToxicGasReading(timestamp, RandomDouble(s.MinPpm, s.MaxPpm));
    }

    private CombustibleGasReading GenerateCombustibleGas(DateTimeOffset timestamp)
    {
        var s = _settings.CombustibleGas;
        return new CombustibleGasReading(timestamp, RandomDouble(s.MinLeL, s.MaxLeL));
    }

    private PhotoionizationReading GeneratePhotoionization(DateTimeOffset timestamp)
    {
        var s = _settings.Photoionization;
        return new PhotoionizationReading(timestamp, RandomDouble(s.MinPpm, s.MaxPpm));
    }

    private LevelReading GenerateLevel(DateTimeOffset timestamp)
    {
        var s = _settings.Level;
        return new LevelReading(timestamp, RandomDouble(s.MinMeters, s.MaxMeters));
    }

    private MassFlowReading GenerateMassFlow(DateTimeOffset timestamp)
    {
        var s = _settings.MassFlow;
        return new MassFlowReading(timestamp, RandomDouble(s.MinKgH, s.MaxKgH));
    }

    private LoadCellReading GenerateLoadCell(DateTimeOffset timestamp)
    {
        var s = _settings.LoadCell;
        return new LoadCellReading(timestamp, RandomDouble(s.MinKg, s.MaxKg));
    }

    private double RandomDouble(double min, double max) =>
        min + (_random.NextDouble() * (max - min));
}
