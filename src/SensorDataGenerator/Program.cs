using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using SensorDataGenerator.Application.Messaging;
using SensorDataGenerator.Application.Output;
using SensorDataGenerator.Application.Services;
using SensorDataGenerator.Application.Health;
using SensorDataGenerator.Configuration;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Environment.ContentRootPath = AppContext.BaseDirectory;

    builder.Configuration
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.Services.AddSerilog((services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();
    });

    builder.Services.AddSingleton<ISensorPayloadMapper, SensorPayloadMapper>();

    builder.Services.AddHealthChecks()
        .AddCheck<MqttHealthCheck>("mqtt", tags: ["mqtt", "ready"]);

    builder.Services.Configure<SensorSettings>(
        builder.Configuration.GetSection("SensorSettings"));

    builder.Services.AddSingleton<IMessageFormatter>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<SensorSettings>>().Value;
        return settings.Output.Format.Equals("Xml", StringComparison.OrdinalIgnoreCase)
            ? new XmlMessageFormatter()
            : new JsonMessageFormatter();
    });

    builder.Services
        .AddOptions<MqttSettings>()
        .Bind(builder.Configuration.GetSection(MqttSettings.SectionName))
        .ValidateOnStart();

    builder.Services.AddSingleton<IValidateOptions<MqttSettings>, MqttSettingsValidator>();

    builder.Services.AddSingleton<MqttOutputSink>();

    builder.Services.AddSingleton<IEnumerable<IOutputSink>>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<SensorSettings>>().Value;
        var sinks = new List<IOutputSink>();

        if (settings.Output.ConsoleEnabled)
            sinks.Add(new ConsoleOutputSink());

        if (settings.Output.FileEnabled)
        {
            sinks.Add(new FileOutputSink(
                settings.Output.FilePath,
                settings.Output.RetentionDays,
                settings.Output.RetentionHours));
        }

        var mqtt = sp.GetRequiredService<IOptions<MqttSettings>>().Value;
        if (mqtt.Enabled)
        {
            sinks.Add(sp.GetRequiredService<MqttOutputSink>());
        }

        return sinks;
    });

    builder.Services.AddHostedService<SensorSimulationService>();

    var app = builder.Build();

    app.MapHealthChecks("/healthz", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var json = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    data = e.Value.Data,
                    duration = e.Value.Duration.ToString()
                })
            });
            await context.Response.WriteAsync(json);
        }
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
