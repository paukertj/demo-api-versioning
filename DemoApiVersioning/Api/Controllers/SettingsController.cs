using Api.Abstractions;
using Api.Abstractions.Dtos;
using Core.Abstractions.Domains;
using Core.Abstractions.Services;
using Majipro.Converter;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route(ISettingsContract.Route)]
public sealed class SettingsController : ControllerBase, ISettingsContract
{
    private readonly ISettingsService _settingsService;
    private readonly IConvertingService _convertingService;
    
    public SettingsController(ISettingsService settingsService, IConvertingService convertingService)
    {
        _settingsService = settingsService;
        _convertingService = convertingService;
    }

    [HttpGet]
    public async Task<SettingsDto> GetSettingsAsync([FromQuery] string key, [FromQuery] int valueVersion)
    {
        var settingsDomain = await _settingsService.GetSettingsAsync(key, valueVersion, ISettingsContract.SchemaVersion, HttpContext.RequestAborted);
        
        var settingsDto = _convertingService.Convert<SettingsDomain, SettingsDto>(settingsDomain);
        
        return settingsDto;
    }

    [HttpPost]
    public async Task ManageSettingsAsync([FromBody] SettingsItemDto dto)
    {
        var settingsDomain = _convertingService.Convert<SettingsItemDto, SettingsItemDomain>(dto);
        
        await _settingsService.ManageSettingsAsync(settingsDomain, HttpContext.RequestAborted);
    }

    [HttpDelete]
    public async Task DeleteSettingsAsync([FromQuery] string key, [FromQuery] int valueVersion)
    {
        await _settingsService.DeleteSettingsAsync(key, valueVersion, ISettingsContract.SchemaVersion, HttpContext.RequestAborted);
    }
}
