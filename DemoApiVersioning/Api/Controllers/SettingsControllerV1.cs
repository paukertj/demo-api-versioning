using Api.Abstractions.Dtos;
using Api.Abstractions.V1;
using Core.Abstractions.Domains;
using Core.Abstractions.Domains.V1;
using Core.Abstractions.Services;
using Majipro.Converter;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route(ISettingsContractV1.Route)]
public sealed class SettingsControllerV1 : ControllerBase, ISettingsContractV1
{
    private readonly ISettingsService _settingsService;
    private readonly IConvertingService _convertingService;
    
    public SettingsControllerV1(ISettingsService settingsService, IConvertingService convertingService)
    {
        _settingsService = settingsService;
        _convertingService = convertingService;
    }

    [HttpGet]
    public async Task<SettingsDto<SettingsValueV1Dto>>GetSettingsAsync([FromQuery] string key, [FromQuery] int valueVersion)
    {
        var settingsDomain = await _settingsService.GetSettingsAsync<SettingsValueV1Domain>(key, valueVersion, ISettingsContractV1.SchemaVersion, HttpContext.RequestAborted);
        
        var settingsDto = _convertingService.Convert<SettingsDomain<SettingsValueV1Domain>, SettingsDto<SettingsValueV1Dto>>(settingsDomain);
        
        return settingsDto;
    }

    [HttpPost]
    public async Task ManageSettingsAsync([FromBody] SettingsDto<SettingsValueV1Dto> dto)
    {
        var settingsDomain = _convertingService.Convert<SettingsDto<SettingsValueV1Dto>, SettingsDomain<SettingsValueV1Domain>>(dto);
        
        await _settingsService.ManageSettingsAsync(settingsDomain, ISettingsContractV1.SchemaVersion, HttpContext.RequestAborted);
    }

    [HttpDelete]
    public async Task DeleteSettingsAsync([FromQuery] string key, [FromQuery] int valueVersion)
    {
        await _settingsService.DeleteSettingsAsync(key, valueVersion, ISettingsContractV1.SchemaVersion, HttpContext.RequestAborted);
    }   
}
