namespace Trilha.DotNet.Repository.Frameworks;

public class MongoRepository<T>(IMongoClient mongoClient, string databaseName) where T : class, IMongoObjectBase
{
    private IMongoDatabase MongoDatabase { get; } = mongoClient.GetDatabase(databaseName);

    public IMongoCollection<T> Collection => MongoDatabase.GetCollection<T>(nameof(T));

    /// <summary>
    /// Executa uma consulta em uma coleção do MongoDB, aplicando filtros e projeções especificados.
    /// </summary>
    /// <param name="filter">Uma lista de filtros a serem aplicados à consulta.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Uma lista de documentos BSON que correspondem à consulta.</returns>
    protected async Task<List<TResult>?> Get<TResult>(FilterDefinition<TResult>? filter = null) where TResult : class
    {
        var collection = MongoDatabase.GetCollection<TResult>(nameof(T));

        if (collection == null)
            throw new InvalidOperationException($"Failed to retrieve collection for type {typeof(T).Name}.");

        try
        {
            var projection = MongoExtensions.GenerateProjection<T, TResult>();
            var finalFilter = filter ?? Builders<TResult>.Filter.Empty;
            return await collection.Find(finalFilter)
                .Project<TResult>(projection)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error occurred while querying collection for type {typeof(T).Name}.", ex);
        }
    }
}