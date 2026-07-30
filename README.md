# SensorDataGenerator

A .NET 10 sensor data simulator for testing IoT pipelines without real hardware.
Generates realistic readings for 15+ industrial and environmental sensor types
(weather, pH, ORP, dissolved oxygen, toxic gas, load cell, spectroscopic/FTIR,
and more) and streams them to console, file, or MQTT.

Useful for testing MQTT brokers, Grafana dashboards, Kafka consumers, or any
downstream pipeline that expects sensor telemetry — no physical sensors required.

## Example Output

```json
{
  "sensorId": "ph-01",
  "sensorType": "Ph",
  "timestamp": "2026-07-30T14:22:01Z",
  "value": 7.42,
  "unit": "pH"
}
```

## Features

- 15+ sensor types with configurable, realistic bounds
- Console, file, and MQTT output — all three can run simultaneously
- MQTT sink with bounded-channel buffering, auto-reconnect, and backpressure handling
- Built-in `/healthz` endpoint reporting Healthy / Degraded / Unhealthy states
- Docker and docker-compose ready

## Quick Start

```bash
# Local (.NET 10 SDK required)
dotnet run --project src/SensorDataGenerator

# Docker (set MQTT connection in .env)
echo 'MQTT_PASSWORD=your-secret' > .env
docker compose up -d

# Verify
curl http://localhost:5000/healthz
```

## Configuration

All settings live in `appsettings.json` and can be overridden by environment variables
(colon `:` or double-underscore `__` separator).

### Key Environment Variables

| Variable | Default | Config Path |
|---|---|---|
| `SensorSettings__IntervalMilliseconds` | `2000` | Generation interval |
| `SensorSettings__Output__Format` | `Json` | `Json` or `Xml` |
| `SensorSettings__Output__ConsoleEnabled` | `true` | Print messages to stdout |
| `SensorSettings__Output__FileEnabled` | `false` | Write messages to disk |
| `SensorSettings__Output__Mqtt__Enabled` | `true` | Enable MQTT |
| `SensorSettings__Output__Mqtt__Host` | `10.10.1.10` | MQTT broker address |
| `SensorSettings__Output__Mqtt__Port` | `1883` | MQTT broker port |
| `SensorSettings__Output__Mqtt__Username` | `integrationlab` | Broker username |
| `SensorSettings__Output__Mqtt__Password` | *(empty)* | Broker password |
| `SensorSettings__Output__Mqtt__ClientId` | `SensorDataGenerator` | MQTT client ID |
| `SensorSettings__Output__Mqtt__TopicPrefix` | `sensors` | Topic = `{prefix}/{sensorId}` |
| `SensorSettings__Output__Mqtt__UseTls` | `false` | Enable TLS |
| `SensorSettings__Output__Mqtt__KeepAliveSeconds` | `30` | MQTT keepalive |
| `SensorSettings__Output__Mqtt__ReconnectDelaySeconds` | `5` | Delay between reconnect attempts |

The `docker-compose.yml` uses shorter variable names (e.g. `MQTT_HOST`) that map to the
full config paths above.

### Sensor Bounds

Each sensor type has configurable min/max bounds. For example:

```json
{
  "SensorSettings": {
    "Weather": { "MinTemp": 15.0, "MaxTemp": 32.0 },
    "Sound": { "MinDb": 30.0, "MaxDb": 95.0 },
    "Ph": { "MinPh": 0.0, "MaxPh": 14.0 }
  }
}
```

See `appsettings.json` for all defaults.

## Sensor Types

| Type | Reading | Units |
|---|---|---|
| `Weather` | Temperature, Humidity, Pressure | C, %, hPa |
| `Sound` | Decibals | dB |
| `Electrical` | Voltage, Current, Power | V, A, W |
| `Ph` | pH | unitless |
| `Orp` | ORP | mV |
| `Conductivity` | Conductivity | mS/cm |
| `ChemicalConcentration` | Concentration | % |
| `Spectroscopic` | Wavenumber/Intensity arrays (simulated FTIR/NIR) | cm^-1 |
| `DissolvedOxygen` | Dissolved oxygen | ppm |
| `ToxicGas` | Toxic gas concentration | ppm |
| `CombustibleGas` | LEL | % |
| `Photoionization` | VOCs (PID) | ppm |
| `Level` | Distance | meters |
| `MassFlow` | Flow rate | kg/h |
| `LoadCell` | Weight | kg |

The `Spectroscopic` sensor generates simulated spectral data with 4 Gaussian peaks,
random baseline, and noise.

## Output Modes

| Mode | Config | Description |
|---|---|---|
| **Console** | `ConsoleEnabled` | Writes formatted message + separator to stdout |
| **File** | `FileEnabled` | `{FilePath}/{SensorType}/{yyyy-MM-dd}/{HH}/{SensorId}_{mmss_fff}.{json/xml}`. Cleans up files older than retention window on startup |
| **MQTT** | `Mqtt.Enabled` | Publishes to `{TopicPrefix}/{sensorId}` (QoS 1). Uses a bounded channel buffer (capacity 10,000) with auto-reconnect |

All three modes can be active simultaneously.

## Health Check

```
GET /healthz
```

Returns an aggregate health report:

```json
{
  "status": "Healthy",
  "checks": [{
    "name": "mqtt",
    "status": "Healthy",
    "description": "MQTT healthy",
    "data": {
      "IsConnected": true,
      "LastSuccessfulWrite": "...",
      "LastError": "...",
      "ConsecutiveErrorCount": 0,
      "BufferCount": 0,
      "BufferCapacity": 10000,
      "MessagesDropped": 0
    },
    "duration": "00:00:00.0001234"
  }]
}
```

**MQTT statuses:**
- **Healthy** — Connected & publishing, or MQTT disabled, or startup grace period (first 30s)
- **Degraded** — Buffer >80% full, disconnected with buffered messages, or connected with consecutive publish failures
- **Unhealthy** — Messages dropped (buffer full), or disconnected with no buffered data

The Docker Compose file configures a healthcheck probe (`curl /healthz`) every 15s.

## Build & Test

```bash
dotnet restore
dotnet build         # Build only
dotnet test          # Run tests (xUnit + FluentAssertions)
```

## Project Structure

```
src/SensorDataGenerator/
├── Application/         # Simulation service, output sinks, message formatters, health
│   ├── Health/
│   ├── Messaging/
│   └── Output/
├── Configuration/       # Settings POCOs + validation
├── Domain/              # Sensor reading models (immutable records)
├── Program.cs           # Entry point, DI composition root
└── appsettings.json     # Default configuration

tests/SensorDataGenerator.Tests/
├── JsonMessageFormatterTests.cs
└── SensorPayloadMappingTests.cs
```
