namespace Trilha.DotNet.Repository.Frameworks;

public class AdoNetRepository
{
    /// <summary>
    /// Método para executar comandos SQL sem retorno (Insert, Update, Delete)
    /// </summary>
    /// <param name="connection">Conexão ao banco de dados</param>
    /// <param name="commandText">Texto do comando SQL</param>
    /// <param name="parameters">Parâmetros opcionais para o comando SQL</param>
    /// <returns>Quantidade de linhas afetadas</returns>
    protected int ExecuteNonQuery(
        IDbConnection connection, string commandText, params IDataParameter[] parameters)
    {
        EnsureConnectionOpen(connection);

        using var command = connection.CreateCommand();
        command.CommandText = commandText;

        AddParameters(command, parameters);

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Método para executar comandos SQL que retornam um único valor (por exemplo, COUNT, SUM, etc)
    /// </summary>
    /// <param name="connection">Conexão ao banco de dados</param>
    /// <param name="commandText">Texto do comando SQL</param>
    /// <param name="parameters">Parâmetros opcionais para o comando SQL</param>
    /// <returns>Valor retornado pela consulta</returns>
    protected object? ExecuteScalar(
        IDbConnection connection, string commandText, params IDataParameter[] parameters)
    {
        EnsureConnectionOpen(connection);

        using var command = connection.CreateCommand();
        command.CommandText = commandText;

        AddParameters(command, parameters);

        return command.ExecuteScalar();
    }

    /// <summary>
    /// Método para executar comandos SQL e retornar dados como um IDataReader
    /// </summary>
    /// <param name="connection">Conexão ao banco de dados</param>
    /// <param name="commandText">Texto do comando SQL</param>
    /// <param name="parameters">Parâmetros opcionais para o comando SQL</param>
    /// <returns>IDataReader contendo os resultados</returns>
    protected IDataReader ExecuteDataReader(
        IDbConnection connection, string commandText, params IDataParameter[] parameters)
    {
        EnsureConnectionOpen(connection);

        var command = connection.CreateCommand();
        command.CommandText = commandText;

        AddParameters(command, parameters);

        return command.ExecuteReader();
    }

    /// <summary>
    /// Método para garantir que a conexão esteja aberta
    /// </summary>
    /// <param name="connection"></param>
    private static void EnsureConnectionOpen(IDbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
            connection.Open();
    }

    /// <summary>
    /// Método para adicionar parâmetros ao comando, se existirem
    /// </summary>
    /// <param name="command"></param>
    /// <param name="parameters"></param>
    private static void AddParameters(IDbCommand command, IDataParameter[] parameters)
    {
        if (parameters.Length <= 0) return;

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }
    }
}