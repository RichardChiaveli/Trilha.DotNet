namespace Trilha.DotNet.Repository;

public sealed class DapperRepository
{
    private readonly string _provider;
    private readonly string _connectionString;

    public DapperRepository(string provider, string connectionString)
    {
        _provider = provider;
        _connectionString = connectionString;
    }

    public IDbConnection GetDbConnection()
    {
        var dbFactory = DbProviderFactories.GetFactory(_provider);

        var conn = dbFactory.CreateConnection() ??
                   throw new ArgumentException("Invalid provider name");

        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new ArgumentException("Invalid connection string");

        conn.ConnectionString = _connectionString;

        return conn;
    }

    public IDbTransaction BeginTransactionScope()
    {
        var conn = GetDbConnection();

        if (conn.State != ConnectionState.Open)
            conn.Open();

        return conn.BeginTransaction();
    }

    public static void Map<T>()
    {
        var typeClass = typeof(T);

        if (!typeClass.IsClass) return;
        var map = new CustomPropertyTypeMap(typeClass, (type, columnName)
            => type.GetAttributeFromAnnotattion<ColumnAttribute>(columnName));

        SqlMapper.SetTypeMap(typeClass, map);
    }

    public static IList<DataRow> ConvertToDataRowList(IDataReader reader)
    {
        var data = new DataTable();
        data.Load(reader);

        return (from DataRow row in data.Rows select row).ToList();
    }

    public static void Destroy(IDbConnection conn)
    {
        conn.Close();
        conn.Dispose();
    }
}