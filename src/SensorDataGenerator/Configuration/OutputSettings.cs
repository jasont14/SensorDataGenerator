namespace SensorDataGenerator.Configuration;

/// <summary>
/// Configuration controlling message format and output destinations.
/// </summary>
public class OutputSettings
{
    public string Format { get; set; } = "Json";
    public bool ConsoleEnabled { get; set; } = true;
    public bool FileEnabled { get; set; } = false;
    public string FilePath { get; set; } = "sensor-output";

    public int RetentionDays { get; set; } = 0;
    public int RetentionHours { get; set; } = 48;
    public MqttSettings Mqtt { get; set; } = new();
}

