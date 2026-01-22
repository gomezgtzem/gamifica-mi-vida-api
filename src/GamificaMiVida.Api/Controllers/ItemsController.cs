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
