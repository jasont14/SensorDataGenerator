namespace SensorDataGenerator.Domain.Models;

public record ToxicGasReading(DateTimeOffset Timestamp, double ToxicGasPpm);
