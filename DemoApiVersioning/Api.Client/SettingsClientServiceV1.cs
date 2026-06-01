using System.Net.Http.Json;
using Api.Abstractions.Dtos;
using Api.Abstractions.V1;

namespace Api.Client;

internal sealed class SettingsClientServiceV1 : ISettingsContractV1
{
    private readonly HttpClient _httpClient;

    public SettingsClientServiceV1(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SettingsDto<SettingsValueV1Dto>> GetSettingsAsync(string key, int valueVersion)
    {
        var url = $"{ISettingsContractV1.Route}?key={Uri.EscapeDataString(key)}&valueVersion={valueVersion}";

        var settings = await _httpClient.GetFromJsonAsync<SettingsDto<SettingsValueV1Dto>>(url);

        if (settings == null)
        {
            throw new InvalidOperationException($"No settings returned for key '{key}', value version {valueVersion}.");
        }

        return settings;
    }

    public async Task ManageSettingsAsync(SettingsDto<SettingsValueV1Dto> dto)
    {
        var response = await _httpClient.PostAsJsonAsync(ISettingsContractV1.Route, dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSettingsAsync(string key, int valueVersion)
    {
        var url = $"{ISettingsContractV1.Route}?key={Uri.EscapeDataString(key)}&valueVersion={valueVersion}";

        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
