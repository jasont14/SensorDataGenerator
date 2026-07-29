namespace SensorDataGenerator.Domain.Models;

/// <summary>
/// Domain model representing a single weather sensor reading.
/// </summary>
public record WeatherReading(
    DateTimeOffset Timestamp,
    double TemperatureCelsius,
    double HumidityPercent,
    double PressureHpa
)
{
    public override string ToString() => 
    $"Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss} | Temperature: {TemperatureCelsius, 6 :F1}  | Humidity: {HumidityPercent,5:F1} % | Pressure: {PressureHpa,7:F1} Hpa";
}