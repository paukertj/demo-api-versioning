using Core.Abstractions.Domains;

namespace Infrastructure.Database.Repositories.Settings;

public interface ISettingsRepository
{
    Task<SettingsDomain> GetSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken);
    
    Task ManageSettingsAsync(SettingsItemDomain settingsDomain, CancellationToken cancellationToken);
    
    Task DeleteSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken);
}
