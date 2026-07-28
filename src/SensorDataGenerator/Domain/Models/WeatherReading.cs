namespace SensorDataGenerator.Domain.Models;

public record WeatherReading(
    DateTimeOffset Timestamp,
    double TemperatureCelsius,
    double HumidityPercent,
    double PressureKpa
)
{
    public override string ToString() => 
    $"Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss} | Temperature: {TemperatureCelsius, 6 :F1}  | Humidity: {HumidityPercent,5:F1} % | Pressure: {PressureKpa,7:F1} hPa";
}