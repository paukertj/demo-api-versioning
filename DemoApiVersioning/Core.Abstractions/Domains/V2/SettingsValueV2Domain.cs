namespace Core.Abstractions.Domains.V2;

public sealed record SettingsValueV2Domain
{
    public int IntValue { get; set; }

    public bool BoolValue { get; set; }
}
