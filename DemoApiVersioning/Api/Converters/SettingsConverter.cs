using Api.Abstractions.Dtos;
using Core.Abstractions.Domains;
using Majipro.Converter;

namespace Api.Converters;

internal sealed class SettingsDomainToSettingsDto : IConverter<SettingsDomain, SettingsDto>
{
    private readonly IConvertingService _convertingService;

    public SettingsDomainToSettingsDto(IConvertingService convertingService)
    {
        _convertingService = convertingService;
    }

    public SettingsDto Convert(SettingsDomain from)
    {
        return new SettingsDto
        {
            Items = _convertingService
                .Convert<SettingsItemDomain, SettingsItemDto>(from.Items)
                .ToList()
        };
    }
}

internal sealed class SettingsDtoToSettingsDomain : IConverter<SettingsDto, SettingsDomain>
{
    private readonly IConvertingService _convertingService;

    public SettingsDtoToSettingsDomain(IConvertingService convertingService)
    {
        _convertingService = convertingService;
    }

    public SettingsDomain Convert(SettingsDto from)
    {
        return new SettingsDomain
        {
            Items = _convertingService
                .Convert<SettingsItemDto, SettingsItemDomain>(from.Items)
                .ToList()
        };
    }
}
