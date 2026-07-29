using SensorDataGenerator.Application.Messaging.Dtos;
using SensorDataGenerator.Domain.Models;

namespace SensorDataGenerator.Application.Messaging;

public class SensorPayloadMapper : ISensorPayloadMapper
{
    public WeatherPayloadDto Map(WeatherReading reading) => new()
    {
        TemperatureCelsius = reading.TemperatureCelsius,
        HumidityPercent = reading.HumidityPercent,
        PressureHpa = reading.PressureHpa
    };

    public SoundPayloadDto Map(SoundReading reading) => new()
    {
        Decibels = reading.Decibals
    };

    public ElectricalPayloadDto Map(ElectricalReading reading) => new()
    {
        Volts = reading.Voltage,
        Amps = reading.Current,
        PowerWatts = reading.power
    };

    public PhPayloadDto Map(PhReading reading) => new()
    {
        Ph = reading.Ph
    };

    public OrpPayloadDto Map(OrpReading reading) => new()
    {
        Millivolts = reading.Millivolts
    };

    public ConductivityPayloadDto Map(ConductivityReading reading) => new()
    {
        ConductivityMsCm = reading.ConductivityMsCm
    };

    public ChemicalConcentrationPayloadDto Map(ChemicalConcentrationReading reading) => new()
    {
        ConcentrationPercent = reading.ConcentrationPercent
    };

    public SpectroscopicPayloadDto Map(SpectroscopicReading reading) => new()
    {
        Wavenumbers = reading.Wavenumbers,
        Intensities = reading.Intensities
    };

    public DissolvedOxygenPayloadDto Map(DissolvedOxygenReading reading) => new()
    {
        DoPpm = reading.DoPpm
    };

    public ToxicGasPayloadDto Map(ToxicGasReading reading) => new()
    {
        ToxicGasPpm = reading.ToxicGasPpm
    };

    public CombustibleGasPayloadDto Map(CombustibleGasReading reading) => new()
    {
        LeLPercent = reading.LeLPercent
    };

    public PhotoionizationPayloadDto Map(PhotoionizationReading reading) => new()
    {
        VocsPpm = reading.VocsPpm
    };

    public LevelPayloadDto Map(LevelReading reading) => new()
    {
        DistanceMeters = reading.DistanceMeters
    };

    public MassFlowPayloadDto Map(MassFlowReading reading) => new()
    {
        FlowRateKgH = reading.FlowRateKgH
    };

    public LoadCellPayloadDto Map(LoadCellReading reading) => new()
    {
        WeightKg = reading.WeightKg
    };
}
