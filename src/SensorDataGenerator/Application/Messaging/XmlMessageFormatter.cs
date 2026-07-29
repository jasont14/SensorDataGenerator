using System.Xml.Linq;

namespace SensorDataGenerator.Application.Messaging;

/// <summary>
/// Serializes sensor messages into XML.
/// </summary>
public class XmlMessageFormatter : IMessageFormatter
{
    public string FileExtension => "xml";

    public string Format(SensorMessage message)
    {
        // Structure is intentionally easy to wrap in SOAP Envelope later
        var payloadElement = new XElement("Payload");

        // Simple reflection-based dump of the payload properties
        foreach (var prop in message.Payload.GetType().GetProperties())
        {
            var value = prop.GetValue(message.Payload);
            payloadElement.Add(new XElement(prop.Name, value));
        }

        var doc = new XElement("SensorMessage",
            new XElement("SensorId", message.SensorId),
            new XElement("SensorType", message.SensorType),
            new XElement("Timestamp", message.Timestamp.ToString("O")),
            payloadElement
        );

        return doc.ToString();
    }
}