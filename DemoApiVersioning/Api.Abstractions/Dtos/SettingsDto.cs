namespace Api.Abstractions.Dtos;

public sealed record SettingsDto<T>
{
    public T? Value { get; set; }

    public required string Key { get; set; } = null!;

    public required int ValueVersion { get; set; }
}
