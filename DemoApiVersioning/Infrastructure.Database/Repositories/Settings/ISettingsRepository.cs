namespace Infrastructure.Database.Repositories.Settings;

public interface ISettingsRepository
{
    Task<string?> GetSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken);
    
    Task ManageSettingsAsync(string value, string key, int schemaVersion, int valueVersion, CancellationToken cancellationToken);
    
    Task DeleteSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken);
}
