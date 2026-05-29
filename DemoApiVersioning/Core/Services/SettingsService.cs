using System.ComponentModel.DataAnnotations;
using System.Text;
using Core.Abstractions.Domains;
using Core.Abstractions.Services;
using Core.Exceptions;
using Infrastructure.Database.Repositories.Settings;

namespace Core.Services;

internal sealed class SettingsService : ISettingsService
{
    private readonly ISettingsRepository _settingsRepository;

    public SettingsService(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }
    
    public async Task<SettingsDomain> GetSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken)
    {
        ValidateOrThrow(key, valueVersion, schemaVersion);
        
        var settingsDomains = await _settingsRepository.GetSettingsAsync(key, valueVersion, schemaVersion, cancellationToken);
        
        return settingsDomains;
    }

    public async Task ManageSettingsAsync(SettingsItemDomain settingsDomain, CancellationToken cancellationToken)
    {
        ValidateOrThrow(settingsDomain.Key, settingsDomain.ValueVersion, settingsDomain.SchemaVersion);

        await _settingsRepository.ManageSettingsAsync(settingsDomain, cancellationToken);
    }

    public async Task DeleteSettingsAsync(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken)
    {
        ValidateOrThrow(key, valueVersion, schemaVersion);

        await _settingsRepository.DeleteSettingsAsync(key, valueVersion, schemaVersion, cancellationToken);
    }

    private static void ValidateOrThrow(string key, int valueVersion, int schemaVersion)
    {
        var errors = new StringBuilder();
        
        if (string.IsNullOrWhiteSpace(key))
        {
            errors.AppendLine("Key is required");
        }
        
        if (valueVersion <= 0)
        {
            errors.AppendLine("Value version must be positive integer");
        }
        
        if (schemaVersion <= 0)
        {
            errors.AppendLine("Schema version must be positive integer");
        }

        if (errors.Length > 0)
        {
            throw new DomainValidationException(errors);
        }
    }
}
