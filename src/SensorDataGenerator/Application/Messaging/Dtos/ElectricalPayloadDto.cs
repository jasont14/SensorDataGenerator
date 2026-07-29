namespace SensorDataGenerator.Application.Messaging.Dtos;

/// <summary>
/// Payload DTO for electrical sensor output messages.
/// </summary>
public class ElectricalPayloadDto
{
    public double Volts { get; set; }
    public double Amps { get; set; }
    public double PowerWatts { get; set; }
}