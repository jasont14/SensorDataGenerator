namespace SensorDataGenerator.Configuration;

public class SpectroscopicSettings
{
    public int MinWavenumber { get; set; } = 400;
    public int MaxWavenumber { get; set; } = 4000;
    public int NumPoints { get; set; } = 100;
    public double MinIntensity { get; set; } = 0.0;
    public double MaxIntensity { get; set; } = 1.0;
}
