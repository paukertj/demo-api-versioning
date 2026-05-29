namespace Core.Abstractions.Domains;

public sealed record SettingsDomain
{
    public IReadOnlyList<SettingsItemDomain> Items { get; set; } = Array.Empty<SettingsItemDomain>();
}
