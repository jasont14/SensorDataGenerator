namespace SensorDataGenerator.Configuration;

/// <summary>
/// Configuration bounds used to generate weather readings.
/// </summary>
    public class WeatherSettings
    {
        public double MinTemp { get; set; } = 18.0;
        public double MaxTemp { get; set; } = 28.0;
        public double MinHumidity { get; set; } = 30.0;
        public double MaxHumidity { get; set; } = 70.0;
        public double BasePressure { get; set; } = 1013.25;
    }

     