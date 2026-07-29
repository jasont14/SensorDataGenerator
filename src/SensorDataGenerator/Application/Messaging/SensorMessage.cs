namespace SensorDataGenerator.Application.Messaging;

/// <summary>
/// Represents the envelope for a generated sensor reading message.
/// </summary>
public class SensorMessage
{
    public string SensorId { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
    public object Payload { get; set; } = null!;
}