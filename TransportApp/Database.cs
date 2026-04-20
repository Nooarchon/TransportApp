using Npgsql;
using Dapper;

public class Database
{
    private readonly string _connString;

    public Database(string connString)
    {
        _connString = connString;
    }

    public NpgsqlConnection GetConnection()
        => new NpgsqlConnection(_connString);
}
