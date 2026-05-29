namespace Infrastructure.Database.Entities;

public class SettingsEntity
{
    public string? Value { get; set; }

    public string Key { get; set; } = null!;

    public int SchemaVersion { get; set; }
    
    public int ValueVersion { get; set; }
}
