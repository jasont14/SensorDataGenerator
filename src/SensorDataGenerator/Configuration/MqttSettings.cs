namespace SensorDataGenerator.Configuration;

public class MqttSettings
{
    public const string SectionName = "SensorSettings:Output:Mqtt";

    public bool Enabled { get; set; }
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1883;
    public string ClientId { get; set; } = "SensorDataGenerator";
    public string TopicPrefix { get; set; } = "sensors";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public int KeepAliveSeconds { get; set; } = 30;
    public int ReconnectDelaySeconds { get; set; } = 5;
    public bool UseTls { get; set; } = false;
}