using GamificaMiVida.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace GamificaMiVida.Api.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet("db")]
    public async Task<IActionResult> CheckDb([FromServices] MySqlConnectionFactory factory)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1;";
        var result = await cmd.ExecuteScalarAsync();

        return Ok(new { db = "ok", result });
    }
}
