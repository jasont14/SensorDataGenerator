namespace SensorDataGenerator.Domain.Models;

public record ConductivityReading(DateTimeOffset Timestamp, double ConductivityMsCm);
