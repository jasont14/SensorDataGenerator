namespace SensorDataGenerator.Application.Output;

/// <summary>
/// Writes formatted sensor messages to standard output.
/// </summary>
public class ConsoleOutputSink : IOutputSink
{
    public Task WriteAsync(string formattedMessage, string sensorId, string fileExtension, CancellationToken cancellationToken)
    {
        Console.WriteLine(formattedMessage);
        Console.WriteLine(new string('-', 80));
        return Task.CompletedTask;
    }
}