namespace Trilha.DotNet.Repository.Extensions;

public static class AdoNetExtensions
{
    public static List<T> ToList<T>(this IDataReader reader, Func<IDataReader, T> map)
    {
        var list = new List<T>();
        while (reader.Read())
        {
            list.Add(map(reader));
        }
        return list;
    }

    public static void ExecuteTransaction(this IDbConnection connection, Action<IDbTransaction> action)
    {
        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            action(transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public static void Destroy(this IDbConnection conn)
    {
        conn.Close();
        conn.Dispose();
    }

    public static IDbConnection CreateConnection(this string provider, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("Invalid provider name");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Invalid connection string");

        var dbFactory = DbProviderFactories.GetFactory(provider);
        var conn = dbFactory.CreateConnection() ?? throw new ArgumentException("Invalid provider name");
        conn.ConnectionString = connectionString;

        return conn;
    }
}
