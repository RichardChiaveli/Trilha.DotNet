namespace Trilha.DotNet.Shared.Extensions;

public static class JsonExtensions
{
    public static readonly JsonSerializerOptions DefaultSettings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string Stringify(this object obj)
        => JsonSerializer.Serialize(obj, DefaultSettings);

    public static T ParseJson<T>(this string json)
        => JsonSerializer.Deserialize<T>(json, DefaultSettings)!;
}