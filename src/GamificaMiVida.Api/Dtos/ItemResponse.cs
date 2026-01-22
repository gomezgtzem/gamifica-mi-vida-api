namespace GamificaMiVida.Api.Dtos;

public sealed record ItemResponse(
    string IdHex,
    byte Type,
    string Title,
    string? Notes,
    DateTime? DueAtUtc,
    bool IsCompleted,
    int XpValue
);
