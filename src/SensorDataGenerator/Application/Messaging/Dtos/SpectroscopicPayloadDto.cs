namespace SensorDataGenerator.Application.Messaging.Dtos;

public class SpectroscopicPayloadDto
{
    public double[] Wavenumbers { get; set; } = [];
    public double[] Intensities { get; set; } = [];
}
