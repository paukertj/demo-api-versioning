using Api.Abstractions.Dtos;

namespace Api.Abstractions;

public interface ISettingsContract
{
    const int SchemaVersion = 1;
    
    const string Route = "v1/settings";
    
    Task<SettingsDto> GetSettingsAsync(string key, int valueVersion);

    Task ManageSettingsAsync(SettingsItemDto dto);

    Task DeleteSettingsAsync(string key, int valueVersion);
}
