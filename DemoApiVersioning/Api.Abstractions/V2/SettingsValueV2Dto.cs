namespace Api.Abstractions.V2;

public sealed record SettingsValueV2Dto
{
    public int IntValue { get; set; }
    
    public bool BoolValue { get; set; }
}
