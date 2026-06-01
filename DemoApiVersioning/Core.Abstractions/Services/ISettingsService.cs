using Core.Abstractions.Domains;

namespace Core.Abstractions.Services;

public interface ISettingsService
{
    Task<SettingsDomain> GetSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken);
    
    Task ManageSettingsAsync(SettingsItemDomain settingsDomain, CancellationToken cancellationToken);
    
    Task DeleteSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken);
}
