namespace Api.Abstractions.V1;

public sealed record SettingsValueV1Dto
{
    public string StringValue { get; set; } = string.Empty;
}
