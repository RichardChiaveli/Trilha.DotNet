namespace Trilha.DotNet.Shared.Extensions;

public static class JsonExtensions
{
    public static JsonSerializerOptions Options(this JsonSerializerOptions options)
    {
        options.WriteIndented = false;
        options.AllowTrailingCommas = false;
        options.PropertyNameCaseInsensitive = true;
        options.NumberHandling = JsonNumberHandling.Strict;
        options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        
        return options;
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation
    /// Example (JsonSerializerContext):
    ///     [JsonSourceGenerationOptions(
    ///         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    ///         , IgnoreReadOnlyFields = true
    ///         , IgnoreReadOnlyProperties = true
    ///         , PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
    ///         , WriteIndented = false
    ///         , GenerationMode = JsonSourceGenerationMode.Serialization)]
    ///     [JsonSerializable(typeof(int))]
    ///     public partial class SourceGenerationContext : JsonSerializerContext { }
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TJsonSerializerContext"></typeparam>
    /// <param name="input"></param>
    /// <param name="serializerContext"></param>
    /// <returns></returns>
    public static string Stringify<TInput, TJsonSerializerContext>(this TInput input, TJsonSerializerContext serializerContext)
        where TJsonSerializerContext : JsonSerializerContext =>
        JsonSerializer.Serialize(input, typeof(TInput), serializerContext);

    public static string Stringify<TInput>(this TInput input)
        => JsonSerializer.Serialize(input, new JsonSerializerOptions().Options());

    public static TConvert Deserialize<TConvert, TJsonSerializerContext>(this string input, TJsonSerializerContext serializerContext)
        where TJsonSerializerContext : JsonSerializerContext
    {
        var options = new JsonSerializerOptions().Options();
        var typeInfoResolver = options.TypeInfoResolver;
        options.TypeInfoResolver = serializerContext;

        var result = JsonSerializer.Deserialize<TConvert>(input, options)!;
        options.TypeInfoResolver = typeInfoResolver;

        return result;
    }

    public static TConvert Deserialize<TConvert>(this string input)
        => JsonSerializer.Deserialize<TConvert>(input, new JsonSerializerOptions().Options())!;

    public static TConvert Deserialize<TConvert>(this byte[] input)
        => JsonSerializer.Deserialize<TConvert>(input, new JsonSerializerOptions().Options())!;

    public static string Minify(this string input)
        => Regex.Replace(input, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1",
            RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1)).Replace("//", string.Empty);
}