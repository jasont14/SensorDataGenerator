namespace SensorDataGenerator.Configuration;

/// <summary>
/// Configuration describing a single logical sensor instance.
/// </summary>
public class SensorInstanceSettings
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;   // "Weather", "Sound", "Electrical", "Ph", "Orp", "Conductivity", "ChemicalConcentration", "Spectroscopic", "DissolvedOxygen", "ToxicGas", "CombustibleGas", "Photoionization", "Level", "MassFlow", "LoadCell"
}