namespace Trilha.DotNet.Repository;

public abstract class DapperRepository
{
    public abstract IDbConnection GetDbConnection();

    private static void Map<T>()
    {
        var typeClass = typeof(T);

        if (!typeClass.IsClass) return;
        var map = new CustomPropertyTypeMap(typeClass, (type, columnName)
            => type.GetAttributeFromAnnotattion<ColumnAttribute>(columnName));

        SqlMapper.SetTypeMap(typeClass, map);
    }

    public async Task<IEnumerable<T>> GetBy<T>(string query
        , object? arguments = null
        , CommandType commandType = CommandType.Text
        , IDbTransaction? transaction = null)
    {
        Map<T>();

        using var conn = GetDbConnection();
        return await conn.QueryAsync<T>(query, arguments, transaction, 0, commandType);
    }

    public async Task<IEnumerable<DataRow>> GetBy(string query
        , object? arguments = null
        , CommandType commandType = CommandType.Text
        , IDbTransaction? transaction = null)
    {
        using var conn = GetDbConnection();
        var result = await conn.ExecuteReaderAsync(query, arguments, transaction, 0, commandType);

        var dt = new DataTable();
        dt.Load(result);

        return from DataRow row in dt.Rows select row;
    }

    public async Task<bool> AddUpdateOrDelete(string query
        , object? arguments = null
        , CommandType commandType = CommandType.Text
        , IDbTransaction? transaction = null)
    {
        using var conn = GetDbConnection();
        return await conn.ExecuteAsync(query, arguments, transaction, 0, commandType) > 0;
    }
}