using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace GamificaMiVida.Infrastructure.Database;

public sealed class MySqlConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:MySql");
        var cs = configuration.GetConnectionString("MySql")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:MySql");

        var builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(cs);
        Console.WriteLine($"[DB] Server={builder.Server}; Database={builder.Database}; User={builder.UserID}; Port={builder.Port}");

        _connectionString = cs;

    }

    public MySqlConnection Create() => new MySqlConnection(_connectionString);

}
