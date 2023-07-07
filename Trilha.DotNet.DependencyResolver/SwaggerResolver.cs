namespace Trilha.DotNet.DependencyResolver;

public static class SwaggerResolver
{
    public static IApplicationBuilder UseSwaggerVersion(
        this IApplicationBuilder app
        , IApiVersionDescriptionProvider versionProvider
        , string documentTitle
        , string? css = null)
    {
        app.UseSwagger()
           .UseSwaggerUI(
                options =>
                {
                    foreach (var description in versionProvider.ApiVersionDescriptions)
                    {
                        var groupName = description.GroupName.ToLowerInvariant();

                        options.DocumentTitle = documentTitle;
                        options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName);
                        options.DefaultModelsExpandDepth(-1);

                        if (!string.IsNullOrWhiteSpace(css)) options.InjectStylesheet(css);
                    }
                });

        return app;
    }

    public static IServiceCollection AddSwaggerVersion(
        this IServiceCollection services
        , IApiVersionDescriptionProvider versionProvider
        , OpenApiInfo info)
    {
        services.AddSwaggerGen(options =>
        {
            foreach (var description in versionProvider.ApiVersionDescriptions)
            {
                var groupName = description.GroupName.ToLowerInvariant();
                info.Version = description.ApiVersion.ToString();
                options.SwaggerDoc(groupName, info);
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter the JWT token like this: Bearer {your token}",
                Name = "Authorization",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "Bearer",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            options.OperationFilter<SwaggerHeadersFilter>();

            options.DocumentFilter<SwaggerEnumsFilter>();

            var path =
                Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml");

            if (File.Exists(path))
            {
                options.IncludeXmlComments(path);
            }
        });

        return services;
    }
}
