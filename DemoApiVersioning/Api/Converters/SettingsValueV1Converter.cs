using Api.Abstractions.V1;
using Core.Abstractions.Domains.V1;
using Majipro.Converter;

namespace Api.Converters;

internal sealed class SettingsValueDomainV1Converter : IConverter<SettingsValueV1Domain?, SettingsValueV1Dto?>
{
    public SettingsValueV1Dto? Convert(SettingsValueV1Domain? from)
    {
        if (from is null)
        {
            return null;
        }

        return new SettingsValueV1Dto
        {
            StringValue = from.StringValue
        };
    }
}

internal sealed class SettingsValueDtoV1Converter : IConverter<SettingsValueV1Dto?, SettingsValueV1Domain?>
{
    public SettingsValueV1Domain? Convert(SettingsValueV1Dto? from)
    {
        if (from is null)
        {
            return null;
        }

        return new SettingsValueV1Domain
        {
            StringValue = from.StringValue
        };
    }
}
