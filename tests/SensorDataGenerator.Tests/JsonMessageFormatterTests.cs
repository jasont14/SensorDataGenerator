using System.Text.Json;
using SensorDataGenerator.Application.Messaging;
using SensorDataGenerator.Application.Messaging.Dtos;
using FluentAssertions;
using Xunit;

namespace SensorDataGenerator.Tests;

public class JsonMessageFormatterTests
{
    [Fact]
    public void Format_ProducesExpectedTopLevelAndPayloadShape()
    {
        var timestamp = new DateTimeOffset(2026, 7, 28, 12, 34, 56, TimeSpan.Zero);
        var message = new SensorMessage
        {
            SensorId = "Weather-001",
            SensorType = "Weather",
            Timestamp = timestamp,
            Payload = new WeatherPayloadDto
            {
                TemperatureCelsius = 24.2,
                HumidityPercent = 48.7,
                PressureHpa = 1012.6
            }
        };

        var formatter = new JsonMessageFormatter();
        var json = formatter.Format(message);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        root.TryGetProperty("SensorId", out var sensorId).Should().BeTrue();
        sensorId.GetString().Should().Be("Weather-001");

        root.TryGetProperty("SensorType", out var sensorType).Should().BeTrue();
        sensorType.GetString().Should().Be("Weather");

        root.TryGetProperty("Timestamp", out var timestampElement).Should().BeTrue();
        timestampElement.GetDateTimeOffset().Should().Be(timestamp);

        root.TryGetProperty("Payload", out var payload).Should().BeTrue();
        payload.TryGetProperty("TemperatureCelsius", out var temperature).Should().BeTrue();
        payload.TryGetProperty("HumidityPercent", out var humidity).Should().BeTrue();
        payload.TryGetProperty("PressureHpa", out var pressure).Should().BeTrue();

        temperature.GetDouble().Should().Be(24.2);
        humidity.GetDouble().Should().Be(48.7);
        pressure.GetDouble().Should().Be(1012.6);
    }
}
