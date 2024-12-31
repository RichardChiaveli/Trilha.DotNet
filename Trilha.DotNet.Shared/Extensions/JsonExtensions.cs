namespace Trilha.DotNet.Shared.Extensions;

public static class JsonExtensions
{
    public static JsonSerializerOptions ConfigureSerializeOptions(this JsonSerializerOptions? settings, params JsonConverter[] converters)
    {
        settings ??= new JsonSerializerOptions();

        settings.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        settings.PropertyNameCaseInsensitive = true;
        settings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        settings.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        settings.WriteIndented = false;
        settings.NumberHandling = JsonNumberHandling.AllowReadingFromString;

        if (converters.Length <= 0) return settings;
        
        settings.Converters.Clear();

        foreach (var converter in converters)
        {
            settings.Converters.Add(converter);
        }

        return settings;
    }

    public static string Stringify(this object obj)
        => JsonSerializer.Serialize(obj, ConfigureSerializeOptions(null));

    public static T ParseJson<T>(this string json)
        => JsonSerializer.Deserialize<T>(json, ConfigureSerializeOptions(null))!;
}