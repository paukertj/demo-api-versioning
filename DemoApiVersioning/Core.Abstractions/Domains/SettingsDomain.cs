namespace Core.Abstractions.Domains;

public sealed record SettingsDomain<T>
{
    public T? Value { get; set; }

    public required string Key { get; set; } = null!;
    
    public required int ValueVersion { get; set; }
}
