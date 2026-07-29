namespace SensorDataGenerator.Domain.Models;

public record LevelReading(DateTimeOffset Timestamp, double DistanceMeters);
