namespace Core.Abstractions.Domains.V1;

public sealed record SettingsValueV1Domain
{
    public string StringValue { get; set; } = string.Empty;
}
