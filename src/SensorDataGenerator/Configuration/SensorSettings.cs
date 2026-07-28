namespace SensorDataGenerator.Configuration;

public class SensorSettings()
{
    public int IntervalMilliseconds {get; set;} = 2000;

    public WeatherSettings WeatherSensor {get; set;} = new();
    public ElectricalSettings ElectricalSensor {get; set;} = new();

    public SoundSettings SoundSensor {get; set;} = new();

}

public class WeatherSettings()
{
    public double MinTemp {get; set;} = 18.0;
    public double MaxTemp {get; set;} = 28.0;

    public double MinHumidity {get; set;} = 30.0;
    public double MaxHumidity {get; set;} = 70.0;

    public double BasePressure {get; set; } = 1013.25;   
}

public class ElectricalSettings()
{
    public double NomincalVoltage {get; set;} = 120.0;
    public double VoltageVariance {get; set;} = 5.0;    // +/- 3 Volts

    public double MinAmps {get; set;} = 0.5;
    public double MaxAmps {get; set;} = 12.0; 
}

public class SoundSettings()
{
    public double MinDecibals {get; set; } = 35.0;
    public double MaxDecibals {get; set; } = 85.0;
}