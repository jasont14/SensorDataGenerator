using System.Text.Json;

namespace SensorDataGenerator.Application.Messaging;

/// <summary>
/// Serializes sensor messages into indented JSON.
/// </summary>
public class JsonMessageFormatter : IMessageFormatter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public string FileExtension => "json";

    public string Format(SensorMessage message)
    {
        return JsonSerializer.Serialize(message, Options);
    }
}