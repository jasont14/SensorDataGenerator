namespace SensorDataGenerator.Application.Messaging.Dtos;

/// <summary>
/// Payload DTO for weather sensor output messages.
/// </summary>
public class WeatherPayloadDto
{
    public double TemperatureCelsius { get; set; }
    public double HumidityPercent { get; set; }
    public double PressureHpa { get; set; }
}