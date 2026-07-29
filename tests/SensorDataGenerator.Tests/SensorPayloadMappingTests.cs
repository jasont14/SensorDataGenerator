using FluentAssertions;
using SensorDataGenerator.Application.Messaging;
using SensorDataGenerator.Application.Messaging.Dtos;
using SensorDataGenerator.Domain.Models;
using Xunit;

namespace SensorDataGenerator.Tests;

public class SensorPayloadMapperTests
{
    private readonly ISensorPayloadMapper _mapper = new SensorPayloadMapper();

    [Fact]
    public void Map_WeatherReading_To_WeatherPayloadDto()
    {
        var reading = new WeatherReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            TemperatureCelsius: 21.5,
            HumidityPercent: 57.2,
            PressureHpa: 1009.4);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<WeatherPayloadDto>();
        dto.TemperatureCelsius.Should().Be(21.5);
        dto.HumidityPercent.Should().Be(57.2);
        dto.PressureHpa.Should().Be(1009.4);
    }

    [Fact]
    public void Map_SoundReading_To_SoundPayloadDto()
    {
        var reading = new SoundReading(
            TimeStamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            Decibals: 63.7);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<SoundPayloadDto>();
        dto.Decibels.Should().Be(63.7);
    }

    [Fact]
    public void Map_ElectricalReading_To_ElectricalPayloadDto()
    {
        var reading = new ElectricalReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            Voltage: 119.8,
            Current: 3.2);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<ElectricalPayloadDto>();
        dto.Volts.Should().Be(119.8);
        dto.Amps.Should().Be(3.2);
        dto.PowerWatts.Should().BeApproximately(383.36, 0.001);
    }

    [Fact]
    public void Map_PhReading_To_PhPayloadDto()
    {
        var reading = new PhReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            Ph: 7.35);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<PhPayloadDto>();
        dto.Ph.Should().Be(7.35);
    }

    [Fact]
    public void Map_OrpReading_To_OrpPayloadDto()
    {
        var reading = new OrpReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            Millivolts: 245.0);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<OrpPayloadDto>();
        dto.Millivolts.Should().Be(245.0);
    }

    [Fact]
    public void Map_ConductivityReading_To_ConductivityPayloadDto()
    {
        var reading = new ConductivityReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            ConductivityMsCm: 250.5);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<ConductivityPayloadDto>();
        dto.ConductivityMsCm.Should().Be(250.5);
    }

    [Fact]
    public void Map_ChemicalConcentrationReading_To_ChemicalConcentrationPayloadDto()
    {
        var reading = new ChemicalConcentrationReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            ConcentrationPercent: 42.7);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<ChemicalConcentrationPayloadDto>();
        dto.ConcentrationPercent.Should().Be(42.7);
    }

    [Fact]
    public void Map_SpectroscopicReading_To_SpectroscopicPayloadDto()
    {
        var wavenumbers = new double[] { 400.0, 2200.0, 4000.0 };
        var intensities = new double[] { 0.1, 0.8, 0.2 };
        var reading = new SpectroscopicReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            Wavenumbers: wavenumbers,
            Intensities: intensities);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<SpectroscopicPayloadDto>();
        dto.Wavenumbers.Should().Equal(wavenumbers);
        dto.Intensities.Should().Equal(intensities);
    }

    [Fact]
    public void Map_DissolvedOxygenReading_To_DissolvedOxygenPayloadDto()
    {
        var reading = new DissolvedOxygenReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            DoPpm: 8.4);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<DissolvedOxygenPayloadDto>();
        dto.DoPpm.Should().Be(8.4);
    }

    [Fact]
    public void Map_ToxicGasReading_To_ToxicGasPayloadDto()
    {
        var reading = new ToxicGasReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            ToxicGasPpm: 125.0);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<ToxicGasPayloadDto>();
        dto.ToxicGasPpm.Should().Be(125.0);
    }

    [Fact]
    public void Map_CombustibleGasReading_To_CombustibleGasPayloadDto()
    {
        var reading = new CombustibleGasReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            LeLPercent: 35.0);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<CombustibleGasPayloadDto>();
        dto.LeLPercent.Should().Be(35.0);
    }

    [Fact]
    public void Map_PhotoionizationReading_To_PhotoionizationPayloadDto()
    {
        var reading = new PhotoionizationReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            VocsPpm: 450.5);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<PhotoionizationPayloadDto>();
        dto.VocsPpm.Should().Be(450.5);
    }

    [Fact]
    public void Map_LevelReading_To_LevelPayloadDto()
    {
        var reading = new LevelReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            DistanceMeters: 15.25);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<LevelPayloadDto>();
        dto.DistanceMeters.Should().Be(15.25);
    }

    [Fact]
    public void Map_MassFlowReading_To_MassFlowPayloadDto()
    {
        var reading = new MassFlowReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            FlowRateKgH: 1200.5);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<MassFlowPayloadDto>();
        dto.FlowRateKgH.Should().Be(1200.5);
    }

    [Fact]
    public void Map_LoadCellReading_To_LoadCellPayloadDto()
    {
        var reading = new LoadCellReading(
            Timestamp: new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero),
            WeightKg: 750.0);

        var dto = _mapper.Map(reading);

        dto.Should().BeOfType<LoadCellPayloadDto>();
        dto.WeightKg.Should().Be(750.0);
    }
}
