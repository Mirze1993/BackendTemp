using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace PersistenceOracle;

public class OracleDb : IAsyncDisposable
{
    private static string _connectionString = string.Empty;
    private readonly OracleConnection _connection;
    public IDbConnection Connection => _connection;

    public OracleDb()
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new Exception("Connection not initialized!");

        _connection = new OracleConnection(_connectionString);
    }


    public async ValueTask DisposeAsync()
    {
        if (_connection.State != ConnectionState.Closed)
            await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }

    public static void Init(string user,
        string pass,
        string dbHost,
        string serviceName,
        string port,
        string maxPoolSize,
        string timeout)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrEmpty(_connectionString))
            _connectionString =
                $"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={dbHost})(PORT={port})))(CONNECT_DATA=(SERVER=DEDICATED)(SID={serviceName})));User Id={user};Password={pass};Min Pool Size=1;Max Pool Size={maxPoolSize};Pooling=True;Validate Connection=true;Connection Lifetime=3600;Self Tuning=False;Connection Timeout={timeout};";
    }

    public async Task CheckConnection()
    {
        await ScalerAsync(@"SELECT 1 FROM dual", commandType: CommandType.Text);
    }


    private async Task ConnectionOpenAsync()
    {
        if (_connection.State != ConnectionState.Open) await _connection.OpenAsync();
    }

    private OracleCommand CommandStart(string commandText, List<OracleParameter>? parameters = null,
        CommandType commandType = CommandType.Text)
    { 
        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        command.CommandType = commandType;
        if (parameters != null) command.Parameters.AddRange(parameters.ToArray());
        return command; 
    }

    #region NonQuery

    public async Task<int> NonQueryAsync(string commandText, List<OracleParameter>? parameters = null,
        CommandType commandType = CommandType.Text)
    {
        await using var command = CommandStart(commandText, parameters, commandType);
        await ConnectionOpenAsync();
        return await command.ExecuteNonQueryAsync(); 
    }

    #endregion

    #region Scaller

    public async Task<object?> ScalerAsync(string commandText, List<OracleParameter>? parameters = null,
        CommandType commandType = CommandType.Text)
    {
        await using var command = CommandStart(commandText, parameters, commandType);
        await ConnectionOpenAsync();
        return await command.ExecuteScalarAsync();
    }

    #endregion

    #region Reader

    public async Task<T?> ReaderFistAsync<T>(Func<DbDataReader, T> readMethod, string commandText,
        List<OracleParameter>? parameters = null, CommandType commandType = CommandType.Text)
    {
        await using var command = CommandStart(commandText, parameters, commandType);
        await ConnectionOpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        var r = default(T);
        if (reader.HasRows)
        {
            while (await reader.ReadAsync())
            {
                r = readMethod(reader);
                break;
            }

            await reader.CloseAsync();
        }

        return r;
    }

    public async Task<List<T>> ReaderAsync<T>(Func<DbDataReader, T> readMethod, string commandText,
        List<OracleParameter>? parameters = null, CommandType commandType = CommandType.Text)
    {
        await using var command = CommandStart(commandText, parameters, commandType);
        await ConnectionOpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        var r = new List<T>();
        if (reader.HasRows)
        {
            while (await reader.ReadAsync()) r.Add(readMethod(reader));
            await reader.CloseAsync();
        }

        return r;
    }

    #endregion

    #region SetParameter

    public OracleParameter SetParameter()
    {
        return new OracleParameter();
    }


    public OracleParameter SetParameter(string paramName, object? value)
    {
        return new OracleParameter(paramName, value ?? DBNull.Value);
    }


    public OracleParameter SetParameter(string paramName, object? value, OracleDbType type)
    {
        return new OracleParameter(paramName, value ?? DBNull.Value) { OracleDbType = type };
    }


    public OracleParameter SetParameter(string paramName, object? value, int size)
    {
        return new OracleParameter($"{paramName}", value ?? DBNull.Value) { Size = size };
    }


    public OracleParameter SetReturnParameter()
    {
        return new OracleParameter { Direction = ParameterDirection.ReturnValue };
    }


    public OracleParameter SetReturnParameter(string paramName)
    {
        return new OracleParameter
        {
            ParameterName = paramName,
            Direction = ParameterDirection.ReturnValue
        };
    }

    public OracleParameter SetOutParameter(string paramName, OracleDbType dbType)
    {
        return new OracleParameter
        {
            ParameterName = paramName,
            OracleDbType = dbType,
            Direction = ParameterDirection.Output
        };
    }


    public OracleParameter SetOutParameter(string paramName, OracleDbType dbType, int size)
    {
        return new OracleParameter
        {
            ParameterName = paramName,
            OracleDbType = dbType,
            Direction = ParameterDirection.Output,
            Size = size
        };
    }


    public OracleParameter SetInputOutputParameter(string paramName, object value)
    {
        return new OracleParameter(paramName, value) { Direction = ParameterDirection.InputOutput };
    }

    #endregion

    
}