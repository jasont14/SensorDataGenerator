namespace SensorDataGenerator.Configuration;

/// <summary>
/// Configuration bounds used to generate sound readings.
/// </summary>
public class SoundSettings
{
    public double MinDb { get; set; } = 35.0;
    public double MaxDb { get; set; } = 85.0;
}