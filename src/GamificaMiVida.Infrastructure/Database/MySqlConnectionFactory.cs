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
    }

    public MySqlConnection Create() => new MySqlConnection(_connectionString);
}
