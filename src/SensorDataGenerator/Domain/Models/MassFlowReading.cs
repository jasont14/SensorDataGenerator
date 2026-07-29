namespace SensorDataGenerator.Domain.Models;

public record MassFlowReading(DateTimeOffset Timestamp, double FlowRateKgH);
