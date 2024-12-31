namespace Trilha.DotNet.DependencyResolver;

public static class MongoResolver
{
    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MONGO_CONNECTION_STRING") ?? Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

        return services;
    }
}