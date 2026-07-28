using SensorDataGenerator.Application.Interfaces;
using Microsoft.Extensions.Options;
using SensorDataGenerator.Configuration;

namespace SensorDataGenerator.Application.Services;

public class SensorSimulationService : ISensorSimulationService
{

    private readonly SensorSettings _settings;
    private readonly Random _random;

    public SensorSimulationService(IOptions<SensorSettings> options)
    {
        _settings = options.Value;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Starting sensor simulation...CTRL + C to stop.");
    }
}