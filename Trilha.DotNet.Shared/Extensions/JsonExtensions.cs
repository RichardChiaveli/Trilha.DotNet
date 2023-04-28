namespace Trilha.DotNet.Shared.Extensions;

public static class JsonExtensions
{
    public static JsonSerializerSettings AddDefaultSettings(this JsonSerializerSettings settings)
    {
        settings.Formatting = Formatting.None;
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        settings.DefaultValueHandling = DefaultValueHandling.Include;
        settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        return settings;
    }

    public static string Stringify(this object obj)
        => JsonConvert.SerializeObject(obj, new JsonSerializerSettings().AddDefaultSettings());
    
    public static T ParseJson<T>(this string json)
        => JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings().AddDefaultSettings())!;

    public static T GetValue<T>(this JObject param, string key) where T : IConvertible
    {
        var jToken = param.GetValue(key);
        var obj = Convert.ChangeType(jToken, typeof(T));

        return (T)obj!;
    }
}