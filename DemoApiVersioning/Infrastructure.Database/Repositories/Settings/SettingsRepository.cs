using Core.Abstractions.Domains;
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
    
    public async Task<SettingsDomain> GetSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken)
    {
        var items = await _databaseContext.Settings
            .AsNoTracking()
            .Where(settings => settings.Key == key &&
                               settings.ValueVersion == valueVersion &&
                               settings.SchemaVersion == schemaVersion)
            .Select(settings => new SettingsItemDomain
            {
                Key = settings.Key,
                Value = settings.Value,
                SchemaVersion = settings.SchemaVersion,
                ValueVersion = settings.ValueVersion,
            })
            .ToListAsync(cancellationToken);

        return new SettingsDomain
        {
            Items = items
        };
    }

    public async Task ManageSettingsAsync(SettingsItemDomain settingsDomain, CancellationToken cancellationToken)
    {
        var entity = await _databaseContext.Settings
            .FirstOrDefaultAsync(
                settings => settings.Key == settingsDomain.Key &&
                            settings.ValueVersion == settingsDomain.ValueVersion &&
                            settings.SchemaVersion == settingsDomain.SchemaVersion,
                cancellationToken);

        if (entity is null)
        {
            entity = new SettingsEntity
            {
                Key = settingsDomain.Key,
                ValueVersion = settingsDomain.ValueVersion,
                SchemaVersion = settingsDomain.SchemaVersion,
            };

            _databaseContext.Settings.Add(entity);
        }

        entity.Value = settingsDomain.Value;

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
