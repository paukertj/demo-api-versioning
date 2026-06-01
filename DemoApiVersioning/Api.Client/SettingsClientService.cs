using System.Net.Http.Json;
using Api.Abstractions;
using Api.Abstractions.Dtos;

namespace Api.Client;

internal sealed class SettingsClientService : ISettingsContract
{
    private readonly HttpClient _httpClient;

    public SettingsClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SettingsDto> GetSettingsAsync(string key, int valueVersion)
    {
        var url = $"{ISettingsContract.Route}?key={Uri.EscapeDataString(key)}&valueVersion={valueVersion}";

        var settings = await _httpClient.GetFromJsonAsync<SettingsDto>(url);

        if (settings?.Items.Any() != true)
        {
            throw new InvalidOperationException($"No settings returned for key '{key}', value version {valueVersion}.");
        }

        return settings;
    }

    public async Task ManageSettingsAsync(SettingsItemDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync(ISettingsContract.Route, dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSettingsAsync(string key, int valueVersion)
    {
        var url = $"{ISettingsContract.Route}?key={Uri.EscapeDataString(key)}&valueVersion={valueVersion}";

        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
