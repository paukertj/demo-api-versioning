using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Core.Abstractions.Domains;
using Core.Abstractions.Domains.V1;
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
    
    public async Task<SettingsDomain<T>> GetSettingsAsync<T>(string key, int valueVersion, int schemaVersion, CancellationToken cancellationToken)
    {
        ValidateOrThrow(key, valueVersion, schemaVersion);
        
        var payload = await _settingsRepository.GetSettingsAsync(key, valueVersion, schemaVersion, cancellationToken);

        var settingsDomain = new SettingsDomain<T>
        {
            Key = key,
            ValueVersion = valueVersion,
        };
        
        if (payload is null)
        {
            return settingsDomain;
        }
        
        settingsDomain.Value = JsonSerializer.Deserialize<T>(payload);
        
        return settingsDomain;
    }

    public async Task ManageSettingsAsync<T>(SettingsDomain<T> settingsDomain, int schemaVersion, CancellationToken cancellationToken)
    {
        ValidateOrThrow(settingsDomain.Key, settingsDomain.ValueVersion, schemaVersion);
        
        string payload = JsonSerializer.Serialize(settingsDomain.Value);

        await _settingsRepository.ManageSettingsAsync(payload, settingsDomain.Key, settingsDomain.ValueVersion, schemaVersion, cancellationToken);
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
