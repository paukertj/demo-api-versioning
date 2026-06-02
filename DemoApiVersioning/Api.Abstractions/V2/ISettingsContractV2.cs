using Api.Abstractions.Dtos;

namespace Api.Abstractions.V2;

public interface ISettingsContractV2
{
    const int SchemaVersion = 2;

    const string Route = "v2/settings";

    Task<SettingsDto<SettingsValueV2Dto>> GetSettingsAsync(string key, int valueVersion);

    Task ManageSettingsAsync(SettingsDto<SettingsValueV2Dto> dto);

    Task DeleteSettingsAsync(string key, int valueVersion);
}
