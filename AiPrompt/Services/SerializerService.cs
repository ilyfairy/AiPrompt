using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AiPrompt.Services;

public class SerializerService
{
    public ConfigType ConfigType { get; set; } = ConfigType.Json;

    private readonly JsonSerializerOptions options = new();


    public string SearchPattern => ConfigType switch
    {
        ConfigType.Json => "*.json",
        ConfigType.Yaml => "*.yaml",
        _ => throw new Exception("unknown config type"),
    };


    public SerializerService()
    {
        options.Converters.Add(new JsonStringEnumConverter());
    }

    public string Serialize<T>(T value)
    {
        switch (ConfigType)
        {
            case ConfigType.Json:
                return JsonSerializer.Serialize(value, options);
            case ConfigType.Yaml:
                YamlDotNet.Serialization.Serializer serializer = new();
                return serializer.Serialize(value);
            default:
                throw new Exception("unknown config type");
        }
    }

    public T? Deserialize<T>(string value)
    {
        switch (ConfigType)
        {
            case ConfigType.Json:
                return JsonSerializer.Deserialize<T>(value, options);
            case ConfigType.Yaml:
                YamlDotNet.Serialization.Deserializer deserializer = new();
                return deserializer.Deserialize<T>(value);
            default:
                throw new Exception("unknown config type");
        }
    }
}

public enum ConfigType
{
    Json,
    Yaml,
}