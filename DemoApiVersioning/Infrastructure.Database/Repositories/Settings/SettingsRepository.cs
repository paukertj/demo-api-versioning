using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories.Settings;

internal sealed class SettingsRepository : ISettingsRepository
{
    private readonly DatabaseContext _databaseContext;

    public SettingsRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<string?> GetSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken)
    {
        return await _databaseContext.Settings
            .AsNoTracking()
            .Where(settings => settings.Key == key &&
                               settings.ValueVersion == valueVersion &&
                               settings.SchemaVersion == schemaVersion)
            .Select(settings => settings.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task ManageSettingsAsync(string value, string key, int schemaVersion, int valueVersion, CancellationToken cancellationToken)
    {
        var entity = await _databaseContext.Settings
            .FirstOrDefaultAsync(
                settings => settings.Key == key &&
                            settings.ValueVersion == valueVersion &&
                            settings.SchemaVersion == schemaVersion,
                cancellationToken);

        if (entity is null)
        {
            entity = new SettingsEntity
            {
                Key = key,
                ValueVersion = valueVersion,
                SchemaVersion = schemaVersion,
            };

            _databaseContext.Settings.Add(entity);
        }

        entity.Value = value;

        await _databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken)
    {
        await _databaseContext.Settings
            .Where(settings => settings.Key == key &&
                               settings.ValueVersion == valueVersion &&
                               settings.SchemaVersion == schemaVersion)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
