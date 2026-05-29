namespace Api.Abstractions.Dtos;

public sealed record SettingsItemDto
{
    public string? Value { get; set; }

    public required string Key { get; set; } = null!;

    public required int SchemaVersion { get; set; }
    
    public required int ValueVersion { get; set; }
}
