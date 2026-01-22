namespace GamificaMiVida.Api.Dtos;

public sealed record CreateItemRequest(
    string UserIdHex,
    byte Type,
    string Title,
    string? Notes,
    DateTime? DueAtUtc,
    int XpValue
);
