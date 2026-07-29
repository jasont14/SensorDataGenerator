using SensorDataGenerator.Application.Messaging.Dtos;
using SensorDataGenerator.Domain.Models;

namespace SensorDataGenerator.Application.Messaging;

public interface ISensorPayloadMapper
{
    WeatherPayloadDto Map(WeatherReading reading);
    SoundPayloadDto Map(SoundReading reading);
    ElectricalPayloadDto Map(ElectricalReading reading);
    PhPayloadDto Map(PhReading reading);
    OrpPayloadDto Map(OrpReading reading);
    ConductivityPayloadDto Map(ConductivityReading reading);
    ChemicalConcentrationPayloadDto Map(ChemicalConcentrationReading reading);
    SpectroscopicPayloadDto Map(SpectroscopicReading reading);
    DissolvedOxygenPayloadDto Map(DissolvedOxygenReading reading);
    ToxicGasPayloadDto Map(ToxicGasReading reading);
    CombustibleGasPayloadDto Map(CombustibleGasReading reading);
    PhotoionizationPayloadDto Map(PhotoionizationReading reading);
    LevelPayloadDto Map(LevelReading reading);
    MassFlowPayloadDto Map(MassFlowReading reading);
    LoadCellPayloadDto Map(LoadCellReading reading);
}
