namespace SensorDataGenerator.Configuration;

/// <summary>
/// Configuration bounds used to generate electrical readings.
/// </summary>
public class ElectricalSettings
{
    public double NominalVoltage { get; set; } = 120.0;
    public double VoltageVariance { get; set; } = 3.0;
    public double MinAmps { get; set; } = 0.5;
    public double MaxAmps { get; set; } = 12.0;
}