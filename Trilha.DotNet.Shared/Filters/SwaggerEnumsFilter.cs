namespace Trilha.DotNet.Shared.Filters;

public class SwaggerEnumsFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var (key, value) in swaggerDoc.Components.Schemas.Where(x => x.Value?.Enum?.Count > 0))
        {
            var propertyEnums = value.Enum;
            if (propertyEnums is { Count: > 0 })
            {
                value.Description += DescribeEnum(propertyEnums, key);
            }
        }

        foreach (var pathItem in swaggerDoc.Paths.Values)
        {
            DescribeEnumParameters(pathItem.Operations, swaggerDoc);
        }
    }

    private static void DescribeEnumParameters(IDictionary<OperationType, OpenApiOperation> operations, OpenApiDocument swaggerDoc)
    {
        foreach (var oper in operations)
        {
            var description = new StringBuilder();

            foreach (var param in oper.Value.Parameters)
            {
                if (!swaggerDoc.Components.Schemas.TryGetValue(param.Name, out var value)) continue;

                description.Append(DescribeEnum(value.Enum, param.Name));
                param.Description = description.ToString();
            }
        }
    }

    private static string DescribeEnum(IEnumerable<IOpenApiAny> enums, string proprtyTypeName)
    {
        var enumDescriptions = new List<string>();
        var enumType = GetEnumTypeByName(proprtyTypeName);

        if (enumType == null)
            return string.Empty;

        enumDescriptions.AddRange(from OpenApiInteger enumOption in enums
                                  let enumInt = enumOption.Value
                                  select $"{enumInt} = {((Enum)Enum.ToObject(enumType, enumInt)).GetDescriptionFromAttribute()}");

        return $": {string.Join(", ", enumDescriptions)}";
    }

    private static Type? GetEnumTypeByName(string enumTypeName) =>
        AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => x.Name == enumTypeName);
}