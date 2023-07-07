namespace Trilha.DotNet.Shared.Filters;

public class SwaggerHeadersFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        if (context.MethodInfo.GetCustomAttribute(typeof(SwaggerHeaderAttribute)) is SwaggerHeaderAttribute attribute)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = attribute.Name,
                In = ParameterLocation.Header,
                Description = attribute.Description,
                Schema = context.SchemaGenerator.GenerateSchema(attribute.SchemeType, context.SchemaRepository),
                Required = attribute.IsRequired
            });
        }
    }
}