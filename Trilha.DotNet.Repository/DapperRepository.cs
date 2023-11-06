namespace Trilha.DotNet.Repository;

public abstract class DapperRepository
{
    private readonly string _connectionString;
    private readonly string _providerName;

    protected DapperRepository(string connectionString, string providerName)
    {
        _connectionString = connectionString;
        _providerName = providerName;
    }

    private IDbConnection GetDbConnection()
    {
        var dbFactory = DbProviderFactories.GetFactory(_providerName);

        var conn = dbFactory.CreateConnection() ??
                   throw new ArgumentException("Invalid provider name");

        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new ArgumentException("Invalid connection string");

        conn.ConnectionString = _connectionString;
        conn.Open();

        return conn;
    }

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