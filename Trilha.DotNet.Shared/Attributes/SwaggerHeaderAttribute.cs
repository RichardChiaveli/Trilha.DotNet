namespace Trilha.DotNet.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SwaggerHeaderAttribute(string name, Type schemeType, string description, bool isRequired = false)
    : Attribute
{
    public string Name => name;
    public string Description => description;
    public Type SchemeType => schemeType;
    public bool IsRequired => isRequired;
}