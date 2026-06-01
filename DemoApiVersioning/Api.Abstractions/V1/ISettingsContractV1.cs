using Api.Abstractions.Dtos;

namespace Api.Abstractions.V1;

public interface ISettingsContractV1
{
    const int SchemaVersion = 1;
    
    const string Route = "v1/settings";
    
    Task<SettingsDto<SettingsValueV1Dto>> GetSettingsAsync(string key, int valueVersion);

    Task ManageSettingsAsync(SettingsDto<SettingsValueV1Dto> dto);

    Task DeleteSettingsAsync(string key, int valueVersion);
}
