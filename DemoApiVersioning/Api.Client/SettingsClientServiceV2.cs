using System.Net.Http.Json;
using Api.Abstractions.Dtos;
using Api.Abstractions.V2;

namespace Api.Client;

internal sealed class SettingsClientServiceV2 : ISettingsContractV2
{
    private readonly HttpClient _httpClient;

    public SettingsClientServiceV2(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SettingsDto<SettingsValueV2Dto>> GetSettingsAsync(string key, int valueVersion)
    {
        var url = $"{ISettingsContractV2.Route}?key={Uri.EscapeDataString(key)}&valueVersion={valueVersion}";

        var settings = await _httpClient.GetFromJsonAsync<SettingsDto<SettingsValueV2Dto>>(url);

        if (settings == null)
        {
            throw new InvalidOperationException($"No settings returned for key '{key}', value version {valueVersion}.");
        }

        return settings;
    }

    public async Task ManageSettingsAsync(SettingsDto<SettingsValueV2Dto> dto)
    {
        var response = await _httpClient.PostAsJsonAsync(ISettingsContractV2.Route, dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSettingsAsync(string key, int valueVersion)
    {
        var url = $"{ISettingsContractV2.Route}?key={Uri.EscapeDataString(key)}&valueVersion={valueVersion}";

        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
