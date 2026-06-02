using Api.Abstractions.V2;
using Core.Abstractions.Domains.V2;
using Majipro.Converter;

namespace Api.Converters;

internal sealed class SettingsValueDomainV2Converter : IConverter<SettingsValueV2Domain?, SettingsValueV2Dto?>
{
    public SettingsValueV2Dto? Convert(SettingsValueV2Domain? from)
    {
        if (from is null)
        {
            return null;
        }

        return new SettingsValueV2Dto
        {
            IntValue = from.IntValue,
            BoolValue = from.BoolValue
        };
    }
}

internal sealed class SettingsValueDtoV2Converter : IConverter<SettingsValueV2Dto?, SettingsValueV2Domain?>
{
    public SettingsValueV2Domain? Convert(SettingsValueV2Dto? from)
    {
        if (from is null)
        {
            return null;
        }

        return new SettingsValueV2Domain
        {
            IntValue = from.IntValue,
            BoolValue = from.BoolValue
        };
    }
}
