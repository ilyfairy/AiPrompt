using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using AiPrompt.Models;
using Microsoft.Extensions.Logging;

namespace AiPrompt.Services;

public class AppConfigService(ILogger<AppConfigService> _logger, SerializerService serializerService) : ObservableObject
{
    public readonly string ConfigFileName = "config";
    public static string AppDirectory { get; private set; } = AppContext.BaseDirectory;

    public AppConfig Config { get; set; } = new();

    public string ConfigFileNameExt => serializerService.ConfigType switch
    {
        ConfigType.Json => $"{ConfigFileName}.json",
        ConfigType.Yaml => $"{ConfigFileName}.yaml",
        _ => ConfigFileName,
    };

    public void Save()
    {
        try
        {
            File.WriteAllText(Path.Combine(AppDirectory, $"{ConfigFileName}.yaml"), serializerService.Serialize(Config));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save config");
        }
    }

    public void LoadOrCreate()
    {
        AppConfig? config;
        try
        {
            var path = Path.Combine(AppDirectory, ConfigFileNameExt);
            config = serializerService.Deserialize<AppConfig>(File.ReadAllText(path)) ?? new();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load config");
            config = new();
        }
        Config = config;
    }
}

