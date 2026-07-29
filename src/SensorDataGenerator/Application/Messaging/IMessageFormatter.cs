namespace SensorDataGenerator.Application.Messaging;

/// <summary>
/// Formats sensor messages for output serialization.
/// </summary>
public interface IMessageFormatter
{
    string Format(SensorMessage message);
    string FileExtension { get; }
}