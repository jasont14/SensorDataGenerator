namespace SensorDataGenerator.Domain.Models;

public record SoundReading(

    DateTimeOffset TimeStamp,
    double Decibals
)
{
    public override string ToString() => 
    $"Timestamp: {TimeStamp:yyyy-MM-dd HH:mm:ss} | Decibals: {Decibals, 6 :F1} dB";
}