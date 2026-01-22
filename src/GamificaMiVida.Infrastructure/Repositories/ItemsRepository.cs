using GamificaMiVida.Infrastructure.Database;
using MySql.Data.MySqlClient;

namespace GamificaMiVida.Infrastructure.Repositories;

public sealed class ItemsRepository
{
    private readonly MySqlConnectionFactory _factory;

    public ItemsRepository(MySqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<ItemRow>> GetByUserIdAsync(byte[] userId)
    {
        var results = new List<ItemRow>();

        await using var conn = _factory.Create();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT
  HEX(id) AS id_hex,
  type,
  title,
  notes,
  due_at_utc,
  is_completed,
  xp_value
FROM items
WHERE user_id = @user_id
ORDER BY due_at_utc IS NULL, due_at_utc;
";
        cmd.Parameters.Add(new MySqlParameter("@user_id", MySqlDbType.Binary) { Value = userId });

        await using var reader = await cmd.ExecuteReaderAsync();

        var ordIdHex = reader.GetOrdinal("id_hex");
        var ordType = reader.GetOrdinal("type");
        var ordTitle = reader.GetOrdinal("title");
        var ordNotes = reader.GetOrdinal("notes");
        var ordDue = reader.GetOrdinal("due_at_utc");
        var ordCompleted = reader.GetOrdinal("is_completed");
        var ordXp = reader.GetOrdinal("xp_value");

        while (await reader.ReadAsync())
        {
            results.Add(new ItemRow(
                reader.GetString(ordIdHex),
                reader.GetByte(ordType),
                reader.GetString(ordTitle),
                reader.IsDBNull(ordNotes) ? null : reader.GetString(ordNotes),
                reader.IsDBNull(ordDue) ? null : reader.GetDateTime(ordDue),
                reader.GetBoolean(ordCompleted),
                reader.GetInt32(ordXp)
            ));
        }

        return results;
    }

    public sealed record ItemRow(
        string IdHex,
        byte Type,
        string Title,
        string? Notes,
        DateTime? DueAtUtc,
        bool IsCompleted,
        int XpValue
    );
}
