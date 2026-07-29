// Configuration/MqttSettingsValidator.cs
using Microsoft.Extensions.Options;

namespace SensorDataGenerator.Configuration;

public class MqttSettingsValidator : IValidateOptions<MqttSettings>
{
    public ValidateOptionsResult Validate(string? name, MqttSettings options)
    {
        if (!options.Enabled)
            return ValidateOptionsResult.Success;

        if (string.IsNullOrWhiteSpace(options.Host))
            return ValidateOptionsResult.Fail("Mqtt:Host is required when Enabled=true");

        if (options.Port is < 1 or > 65535)
            return ValidateOptionsResult.Fail("Mqtt:Port must be between 1 and 65535");

        if (string.IsNullOrWhiteSpace(options.ClientId))
            return ValidateOptionsResult.Fail("Mqtt:ClientId is required");

        // Password can be empty if broker allows anonymous, but username without password is usually wrong
        if (!string.IsNullOrWhiteSpace(options.Username) && string.IsNullOrWhiteSpace(options.Password))
            return ValidateOptionsResult.Fail("Mqtt:Password is required when Username is set");

        return ValidateOptionsResult.Success;
    }
}