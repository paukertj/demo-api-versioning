using Api.Abstractions.Dtos;
using Api.Abstractions.V2;
using Core.Abstractions.Domains;
using Core.Abstractions.Domains.V2;
using Core.Abstractions.Services;
using Majipro.Converter;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route(ISettingsContractV2.Route)]
public sealed class SettingsControllerV2 : ControllerBase, ISettingsContractV2
{
    private readonly ISettingsService _settingsService;
    private readonly IConvertingService _convertingService;

    public SettingsControllerV2(ISettingsService settingsService, IConvertingService convertingService)
    {
        _settingsService = settingsService;
        _convertingService = convertingService;
    }

    [HttpGet]
    public async Task<SettingsDto<SettingsValueV2Dto>> GetSettingsAsync([FromQuery] string key, [FromQuery] int valueVersion)
    {
        var settingsDomain = await _settingsService.GetSettingsAsync<SettingsValueV2Domain>(key, valueVersion, ISettingsContractV2.SchemaVersion, HttpContext.RequestAborted);

        var settingsDto = _convertingService.Convert<SettingsDomain<SettingsValueV2Domain>, SettingsDto<SettingsValueV2Dto>>(settingsDomain);

        return settingsDto;
    }

    [HttpPost]
    public async Task ManageSettingsAsync([FromBody] SettingsDto<SettingsValueV2Dto> dto)
    {
        var settingsDomain = _convertingService.Convert<SettingsDto<SettingsValueV2Dto>, SettingsDomain<SettingsValueV2Domain>>(dto);

        await _settingsService.ManageSettingsAsync(settingsDomain, ISettingsContractV2.SchemaVersion, HttpContext.RequestAborted);
    }

    [HttpDelete]
    public async Task DeleteSettingsAsync([FromQuery] string key, [FromQuery] int valueVersion)
    {
        await _settingsService.DeleteSettingsAsync(key, valueVersion, ISettingsContractV2.SchemaVersion, HttpContext.RequestAborted);
    }
}
