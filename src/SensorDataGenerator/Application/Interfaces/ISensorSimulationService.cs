namespace SensorDataGenerator.Application.Interfaces;

public interface ISensorSimulationService
{
    Task RunAsync(CancellationToken cancellationToken);
}