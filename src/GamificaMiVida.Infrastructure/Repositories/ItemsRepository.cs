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

    public async Task<ItemRow> CreateAsync(byte[] userId, byte type, string title, string? notes, DateTime? dueAtUtc, int xpValue)
    {
        var idHex = Guid.NewGuid().ToString("N").ToUpperInvariant();
        var idBytes = Convert.FromHexString(idHex);

        await using var conn = _factory.Create();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO items (id, user_id, type, title, notes, due_at_utc, xp_value)
VALUES (@id, @user_id, @type, @title, @notes, @due_at_utc, @xp_value);
";
        cmd.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Binary) { Value = idBytes });
        cmd.Parameters.Add(new MySqlParameter("@user_id", MySqlDbType.Binary) { Value = userId });
        cmd.Parameters.Add(new MySqlParameter("@type", MySqlDbType.UByte) { Value = type });
        cmd.Parameters.Add(new MySqlParameter("@title", MySqlDbType.VarChar) { Value = title });
        cmd.Parameters.Add(new MySqlParameter("@notes", MySqlDbType.Text) { Value = notes is null ? DBNull.Value : notes });
        cmd.Parameters.Add(new MySqlParameter("@due_at_utc", MySqlDbType.DateTime) { Value = dueAtUtc is null ? DBNull.Value : dueAtUtc });
        cmd.Parameters.Add(new MySqlParameter("@xp_value", MySqlDbType.Int32) { Value = xpValue });

        await cmd.ExecuteNonQueryAsync();

        return new ItemRow(
            idHex,
            type,
            title,
            notes,
            dueAtUtc,
            false,
            xpValue
        );
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
