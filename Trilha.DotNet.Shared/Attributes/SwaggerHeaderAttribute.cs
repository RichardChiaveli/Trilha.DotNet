namespace Trilha.DotNet.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SwaggerHeaderAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }
    public Type SchemeType { get; }
    public bool IsRequired { get; }

    public SwaggerHeaderAttribute(string name, Type schemeType, string description, bool isRequired = false)
    {
        Name = name;
        SchemeType = schemeType;
        Description = description;
        IsRequired = isRequired;
    }
}