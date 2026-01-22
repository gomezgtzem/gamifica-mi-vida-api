using GamificaMiVida.Api.Dtos;
using GamificaMiVida.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GamificaMiVida.Api.Controllers;

[ApiController]
[Route("api/items")]
public sealed class ItemsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ItemResponse>>> Get(
        [FromQuery] string userIdHex,
        [FromServices] ItemsRepository repo)
    {
        if (!TryParseHex16(userIdHex, out var userIdBytes))
            return BadRequest(new { error = "userIdHex must be 32 hex chars (16 bytes)" });

        var items = await repo.GetByUserIdAsync(userIdBytes);

        var response = items.Select(x => new ItemResponse(
            x.IdHex,
            x.Type,
            x.Title,
            x.Notes,
            x.DueAtUtc,
            x.IsCompleted,
            x.XpValue
        )).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ItemResponse>> Create(
        [FromBody] CreateItemRequest req,
        [FromServices] ItemsRepository repo)
    {
        if (!TryParseHex16(req.UserIdHex, out var userIdBytes))
            return BadRequest(new { error = "userIdHex must be 32 hex chars (16 bytes)" });

        var title = (req.Title ?? string.Empty).Trim();
        if (title.Length == 0 || title.Length > 200)
            return BadRequest(new { error = "title is required and must be <= 200 chars" });

        if (req.XpValue < 0)
            return BadRequest(new { error = "xpValue must be >= 0" });

        if (req.Type is < 1 or > 3)
            return BadRequest(new { error = "type must be 1..3" });

        DateTime? dueUtc = NormalizeUtc(req.DueAtUtc);

        var created = await repo.CreateAsync(
            userIdBytes,
            req.Type,
            title,
            req.Notes,
            dueUtc,
            req.XpValue
        );

        var response = new ItemResponse(
            created.IdHex,
            created.Type,
            created.Title,
            created.Notes,
            created.DueAtUtc,
            created.IsCompleted,
            created.XpValue
        );

        return CreatedAtAction(nameof(Get), new { userIdHex = req.UserIdHex }, response);
    }

    private static DateTime? NormalizeUtc(DateTime? dt)
    {
        if (!dt.HasValue) return null;

        return dt.Value.Kind switch
        {
            DateTimeKind.Utc => dt.Value,
            DateTimeKind.Local => dt.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc)
        };
    }

    private static bool TryParseHex16(string hex, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();

        if (string.IsNullOrWhiteSpace(hex) || hex.Length != 32)
            return false;

        try
        {
            bytes = Convert.FromHexString(hex);
            return bytes.Length == 16;
        }
        catch
        {
            return false;
        }
    }
}
