using Api.Abstractions.Dtos;
using Core.Abstractions.Domains;
using Majipro.Converter;

namespace Api.Converters;

internal sealed class SettingsItemDomainToSettingsItemDto : IConverter<SettingsItemDomain, SettingsItemDto>
{
    public SettingsItemDto Convert(SettingsItemDomain from)
    {
        return new SettingsItemDto
        {
            Value = from.Value,
            Key = from.Key,
            SchemaVersion = from.SchemaVersion,
            ValueVersion = from.ValueVersion
        };
    }
}

internal sealed class SettingsItemDtoToSettingsItemDomain : IConverter<SettingsItemDto, SettingsItemDomain>
{
    public SettingsItemDomain Convert(SettingsItemDto from)
    {
        return new SettingsItemDomain
        {
            Value = from.Value,
            Key = from.Key,
            SchemaVersion = from.SchemaVersion,
            ValueVersion = from.ValueVersion
        };
    }
}
