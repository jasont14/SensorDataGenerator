namespace SensorDataGenerator.Application.Output;

public class FileOutputSink : IOutputSink
{
    private readonly string _basePath;
    private readonly int _retentionDays;
    private readonly int _retentionHours;
    private bool _cleanupDone;

    public FileOutputSink(string basePath, int retentionDays = 0, int retentionHours = 48)
    {
        _basePath = basePath;
        _retentionDays = retentionDays;
        _retentionHours = retentionHours;
    }

    public async Task WriteAsync(string formattedMessage, string sensorId, string fileExtension, CancellationToken cancellationToken)
    {
        // Run cleanup only once per process lifetime
        if (!_cleanupDone)
        {
            CleanupOldFiles();
            _cleanupDone = true;
        }

        var sensorType = sensorId.Contains('-')
            ? sensorId[..sensorId.IndexOf('-')]
            : "Unknown";

        var now = DateTime.UtcNow;
        var directory = Path.Combine(
            _basePath,
            sensorType,
            now.ToString("yyyy-MM-dd"),
            now.ToString("HH"));

        Directory.CreateDirectory(directory);

        var fileName = $"{sensorId}_{now:mmss_fff}.{fileExtension}";
        var fullPath = Path.Combine(directory, fileName);

        await File.WriteAllTextAsync(fullPath, formattedMessage, cancellationToken);
    }

    private void CleanupOldFiles()
    {
        if (!Directory.Exists(_basePath))
            return;

        var cutoff = DateTime.UtcNow
            .AddDays(-_retentionDays)
            .AddHours(-_retentionHours);

        foreach (var sensorTypeDir in Directory.GetDirectories(_basePath))
        {
            foreach (var dateDir in Directory.GetDirectories(sensorTypeDir))
            {
                foreach (var hourDir in Directory.GetDirectories(dateDir))
                {
                    if (Directory.GetLastWriteTimeUtc(hourDir) < cutoff)
                    {
                        try
                        {
                            Directory.Delete(hourDir, recursive: true);
                        }
                        catch
                        {
                            // best-effort cleanup
                        }
                    }
                }

                // Remove empty date folders
                if (!Directory.EnumerateFileSystemEntries(dateDir).Any())
                {
                    try { Directory.Delete(dateDir); } catch { }
                }
            }
        }
    }
}