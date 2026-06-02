using Api.Abstractions.Dtos;
using Api.Abstractions.V2;
using Core.Abstractions.Domains;
using Core.Abstractions.Domains.V2;
using Majipro.Converter;

namespace Api.Converters;

internal sealed class SettingsItemDomainToSettingsItemV2Dto : IConverter<SettingsDomain<SettingsValueV2Domain>, SettingsDto<SettingsValueV2Dto>>
{
    private readonly IConvertingService _convertingService;

    public SettingsItemDomainToSettingsItemV2Dto(IConvertingService convertingService)
    {
        _convertingService = convertingService;
    }

    public SettingsDto<SettingsValueV2Dto> Convert(SettingsDomain<SettingsValueV2Domain> from)
    {
        return new SettingsDto<SettingsValueV2Dto>
        {
            Value = _convertingService.Convert<SettingsValueV2Domain?, SettingsValueV2Dto?>(from.Value),
            Key = from.Key,
            ValueVersion = from.ValueVersion
        };
    }
}

internal sealed class SettingsItemV2DtoToSettingsItemDomain : IConverter<SettingsDto<SettingsValueV2Dto>, SettingsDomain<SettingsValueV2Domain>>
{
    private readonly IConvertingService _convertingService;

    public SettingsItemV2DtoToSettingsItemDomain(IConvertingService convertingService)
    {
        _convertingService = convertingService;
    }

    public SettingsDomain<SettingsValueV2Domain> Convert(SettingsDto<SettingsValueV2Dto> from)
    {
        return new SettingsDomain<SettingsValueV2Domain>
        {
            Value = _convertingService.Convert<SettingsValueV2Dto?, SettingsValueV2Domain?>(from.Value),
            Key = from.Key,
            ValueVersion = from.ValueVersion
        };
    }
}
