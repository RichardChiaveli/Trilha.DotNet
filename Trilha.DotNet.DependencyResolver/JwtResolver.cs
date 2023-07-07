namespace Trilha.DotNet.DependencyResolver;

public static class JwtResolver
{
    public static IServiceCollection AddJwt(
        this IServiceCollection services
        , IWebHostEnvironment environment
        , string tokenSignature)
    {
        var signature = tokenSignature.CreateTokenSignature();

        services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(bearerOptions =>
        {
            bearerOptions.RequireHttpsMetadata = true;
            bearerOptions.SaveToken = true;

            var paramsValidation = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signature.Key,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidIssuer = environment.EnvironmentName,
                ValidateLifetime = true,
                LifetimeValidator = (notBefore, expires, _, _) =>
                    notBefore <= DateTime.UtcNow && expires >= DateTime.UtcNow,
                ClockSkew = TimeSpan.Zero
            };

            bearerOptions.TokenValidationParameters = paramsValidation;
        });

        services.AddAuthorization(auth =>
        {
            auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build());
        });

        return services;
    }
}