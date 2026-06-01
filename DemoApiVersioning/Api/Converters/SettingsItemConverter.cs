using Api.Abstractions.Dtos;
using Api.Abstractions.V1;
using Core.Abstractions.Domains;
using Core.Abstractions.Domains.V1;
using Majipro.Converter;

namespace Api.Converters;

internal sealed class SettingsItemDomainToSettingsItemDto : IConverter<SettingsDomain<SettingsValueV1Domain>, SettingsDto<SettingsValueV1Dto>>
{
    private readonly IConvertingService _convertingService;

    public SettingsItemDomainToSettingsItemDto(IConvertingService convertingService)
    {
        _convertingService = convertingService;
    }

    public SettingsDto<SettingsValueV1Dto> Convert(SettingsDomain<SettingsValueV1Domain> from)
    {
        return new SettingsDto<SettingsValueV1Dto>
        {
            Value = _convertingService.Convert<SettingsValueV1Domain?, SettingsValueV1Dto?>(from.Value),
            Key = from.Key,
            ValueVersion = from.ValueVersion
        };
    }
}

internal sealed class SettingsItemDtoToSettingsItemDomain : IConverter<SettingsDto<SettingsValueV1Dto>, SettingsDomain<SettingsValueV1Domain>>
{
    private readonly IConvertingService _convertingService;

    public SettingsItemDtoToSettingsItemDomain(IConvertingService convertingService)
    {
        _convertingService = convertingService;
    }

    public SettingsDomain<SettingsValueV1Domain> Convert(SettingsDto<SettingsValueV1Dto> from)
    {
        return new SettingsDomain<SettingsValueV1Domain>
        {
            Value = _convertingService.Convert<SettingsValueV1Dto?, SettingsValueV1Domain?>(from.Value),
            Key = from.Key,
            ValueVersion = from.ValueVersion
        };
    }
}
