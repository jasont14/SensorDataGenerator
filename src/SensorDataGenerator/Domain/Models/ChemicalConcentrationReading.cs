namespace SensorDataGenerator.Domain.Models;

public record ChemicalConcentrationReading(DateTimeOffset Timestamp, double ConcentrationPercent);
