namespace SensorDataGenerator.Domain.Models;

/// <summary>
/// Domain model representing a single sound sensor reading.
/// </summary>
public record SoundReading(

    DateTimeOffset TimeStamp,
    double Decibals
)
{
    public override string ToString() => 
    $"Timestamp: {TimeStamp:yyyy-MM-dd HH:mm:ss} | Decibals: {Decibals, 6 :F1} dB";
}