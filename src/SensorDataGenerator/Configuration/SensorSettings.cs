namespace SensorDataGenerator.Configuration;

/// <summary>
/// Root application configuration for sensor simulation and output behavior.
/// </summary>
public class SensorSettings
{
    public int IntervalMilliseconds { get; set; } = 2000;

    public List<SensorInstanceSettings> Sensors { get; set; } = new();

    public WeatherSettings Weather { get; set; } = new();
    public SoundSettings Sound { get; set; } = new();
    public ElectricalSettings Electrical { get; set; } = new();
    public PhSettings Ph { get; set; } = new();
    public OrpSettings Orp { get; set; } = new();
    public ConductivitySettings Conductivity { get; set; } = new();
    public ChemicalConcentrationSettings ChemicalConcentration { get; set; } = new();
    public SpectroscopicSettings Spectroscopic { get; set; } = new();
    public DissolvedOxygenSettings DissolvedOxygen { get; set; } = new();
    public ToxicGasSettings ToxicGas { get; set; } = new();
    public CombustibleGasSettings CombustibleGas { get; set; } = new();
    public PhotoionizationSettings Photoionization { get; set; } = new();
    public LevelSettings Level { get; set; } = new();
    public MassFlowSettings MassFlow { get; set; } = new();
    public LoadCellSettings LoadCell { get; set; } = new();

    public OutputSettings Output { get; set; } = new();
}