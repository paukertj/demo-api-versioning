using Core.Abstractions.Domains;

namespace Core.Abstractions.Services;

public interface ISettingsService
{
    Task<SettingsDomain<T>> GetSettingsAsync<T>(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken);
    
    Task ManageSettingsAsync<T>(SettingsDomain<T> settingsDomain, int schemaVersion, CancellationToken cancellationToken);
    
    Task DeleteSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken);
}
