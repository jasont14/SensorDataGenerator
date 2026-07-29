namespace SensorDataGenerator.Domain.Models;

public record SpectroscopicReading(DateTimeOffset Timestamp, double[] Wavenumbers, double[] Intensities);
