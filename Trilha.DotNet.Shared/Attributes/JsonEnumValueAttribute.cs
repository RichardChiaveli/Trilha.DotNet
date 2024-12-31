namespace Trilha.DotNet.Shared.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class JsonEnumValueAttribute(string jsonValue) : Attribute
{
    public static readonly JsonEnumValueAttribute Default = new();

    public string JsonValue => jsonValue;

    public JsonEnumValueAttribute() : this(string.Empty) { }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is JsonEnumValueAttribute other && other.JsonValue == JsonValue;

    public override int GetHashCode() => JsonValue.GetHashCode();

    public override bool IsDefaultAttribute() => Equals(Default);
}
