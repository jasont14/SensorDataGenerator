namespace SensorDataGenerator.Application.Messaging.Dtos;

/// <summary>
/// Payload DTO for sound sensor output messages.
/// </summary>
public class SoundPayloadDto
{
    public double Decibels { get; set; }
}