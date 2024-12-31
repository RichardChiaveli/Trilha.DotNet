namespace Trilha.DotNet.Repository.Extensions;

public static class MongoExtensions
{
    private static IEnumerable<string> GetPropNamesForProjection(this Type type, string prefix = "")
    {
        var propertyPrefix = string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.";

        return type.GetProperties().SelectMany(property =>
        {
            var propertyName = $"{propertyPrefix}{property.Name}";
            var propertyType = property.PropertyType;

            if (propertyType.IsGenericType)
            {
                propertyType = propertyType.GetGenericArguments()[0];
            }

            return propertyType.GetInterfaces().Contains(typeof(IMongoObjectBase))
                ? GetPropNamesForProjection(propertyType, propertyName)
                : [propertyName];
        });
    }

    public static ProjectionDefinition<TResult> GenerateProjection<TSource, TResult>()
        where TSource : class, IMongoObjectBase
    {
        var sourceType = typeof(TSource);
        var propertyNames = sourceType.GetPropNamesForProjection();

        var projection = Builders<TResult>.Projection.Combine(
            propertyNames.Select(name => Builders<TResult>.Projection.Include(name)).ToArray());

        return projection;
    }
}
