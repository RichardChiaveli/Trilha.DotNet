namespace Trilha.DotNet.Repository;

public abstract class DapperRepository
{
    public abstract IDbConnection Connection { get; }

    private static void Map<T>() where T : class
    {
        var typeClass = typeof(T);

        var map = new CustomPropertyTypeMap(typeClass, (type, columnName)
            => type.GetAttributeFromAnnotattion<ColumnAttribute>(columnName));

        SqlMapper.SetTypeMap(typeClass, map);
    }

    public async Task<IEnumerable<TEntity>> GetBy<TEntity>(string query,
        object? arguments = null, CommandType commandType = CommandType.Text, IDbTransaction? transaction = null) where TEntity : class
    {
        Map<TEntity>();
        using var conn = Connection;
        return await conn.QueryAsync<TEntity>(query, arguments, transaction, 0, commandType);
    }

    public async Task<IDataReader> GetBy(string query,
        object? arguments = null, CommandType commandType = CommandType.Text, IDbTransaction? transaction = null)
    {
        using var conn = Connection;
        return await conn.ExecuteReaderAsync(query, arguments, transaction, 0, commandType);
    }

    public async Task<T> GetValue<T>(string query,
        object? arguments = null, CommandType commandType = CommandType.Text, IDbTransaction? transaction = null)
    {
        using var conn = Connection;
        return (await conn.ExecuteScalarAsync<T>(query, arguments, transaction, 0, commandType))!;
    }

    public async Task<bool> CreateUpdateOrDelete(string query,
        object? arguments = null, CommandType commandType = CommandType.Text, IDbTransaction? transaction = null)
    {
        using var conn = Connection;
        return await conn.ExecuteAsync(query, arguments, transaction, 0, commandType) > 0;
    }
}