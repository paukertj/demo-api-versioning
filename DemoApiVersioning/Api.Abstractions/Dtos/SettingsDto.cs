namespace Api.Abstractions.Dtos;

public sealed record SettingsDto
{
    public IReadOnlyList<SettingsItemDto> Items { get; set; } = Array.Empty<SettingsItemDto>();
}
