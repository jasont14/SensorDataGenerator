using SensorDataGenerator.Application.Messaging;

namespace SensorDataGenerator.Application.Output;

/// <summary>
/// Represents a destination for formatted sensor messages.
/// </summary>
public interface IOutputSink
{
    Task WriteAsync(string formattedMessage, string sensorId, string fileExtension, CancellationToken cancellationToken);
}