namespace SensorDataGenerator.Domain.Models;

public record ElectricalReading(
    DateTimeOffset Timestamp,
    double Voltage,
    double Current
)
{
    public double power => Voltage * Current;

    public override string ToString() => 
    $"Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss} | Voltage: {Voltage,6:F1} V | Current: {Current,5:F1} A | Power: {power,7:F1} W";)
}
