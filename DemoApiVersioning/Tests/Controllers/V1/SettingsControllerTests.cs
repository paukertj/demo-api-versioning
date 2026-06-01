using System.Net;
using System.Text.Json;
using Api.Abstractions.Dtos;
using Api.Abstractions.V1;
using Infrastructure.Database.Repositories.Settings;
using Microsoft.Extensions.DependencyInjection;
using Tests.Infrastructure;

namespace Tests.Controllers.V1;

[TestClass]
public sealed class SettingsControllerTests
{
    private const int SchemaVersion = ISettingsContractV1.SchemaVersion;

    [TestMethod]
    public async Task GetSettings_ReturnsMatchingRow_WhenSettingExistsForKeyAndValueVersion()
    {
        var key = $"test-{Guid.NewGuid()}";
        await SeedAsync(key, valueVersion: 1, schemaVersion: SchemaVersion, stringValue: "first");
        await SeedAsync(key, valueVersion: 1, schemaVersion: SchemaVersion + 1, stringValue: "second");

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContractV1>();

        var dto = await client.GetSettingsAsync(key, valueVersion: 1);

        Assert.AreEqual(key, dto.Key);
        Assert.AreEqual(1, dto.ValueVersion);
        Assert.IsNotNull(dto.Value);
        Assert.AreEqual("first", dto.Value!.StringValue);
    }

    [TestMethod]
    public async Task GetSettings_ReturnsNullValue_WhenNoSettingsMatch()
    {
        var key = $"missing-{Guid.NewGuid()}";

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContractV1>();

        var dto = await client.GetSettingsAsync(key, valueVersion: 1);

        Assert.AreEqual(key, dto.Key);
        Assert.IsNull(dto.Value);
    }

    [TestMethod]
    public async Task GetSettings_Throws400_WhenKeyIsBlank()
    {
        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContractV1>();

        var exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(
            () => client.GetSettingsAsync("", valueVersion: 1));
        Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [TestMethod]
    public async Task ManageSettings_CreatesNewSetting_WhenCompositeKeyDoesNotExist()
    {
        var key = $"manage-create-{Guid.NewGuid()}";
        var dto = new SettingsDto<SettingsValueV1Dto>
        {
            Key = key,
            Value = new SettingsValueV1Dto { StringValue = "created-via-client" },
            ValueVersion = 1,
        };

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContractV1>();

        await client.ManageSettingsAsync(dto);

        var stored = await ReadValueAsync(key, valueVersion: 1, schemaVersion: SchemaVersion);
        Assert.IsNotNull(stored);
        Assert.AreEqual("created-via-client", stored!.StringValue);
    }

    [TestMethod]
    public async Task ManageSettings_UpdatesExistingSetting_WhenCompositeKeyMatches()
    {
        var key = $"manage-update-{Guid.NewGuid()}";
        await SeedAsync(key, valueVersion: 1, schemaVersion: SchemaVersion, stringValue: "original");

        var dto = new SettingsDto<SettingsValueV1Dto>
        {
            Key = key,
            Value = new SettingsValueV1Dto { StringValue = "updated" },
            ValueVersion = 1,
        };

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContractV1>();

        await client.ManageSettingsAsync(dto);

        var stored = await ReadValueAsync(key, valueVersion: 1, schemaVersion: SchemaVersion);
        Assert.IsNotNull(stored);
        Assert.AreEqual("updated", stored!.StringValue);
    }

    [TestMethod]
    public async Task ManageSettings_Throws400_WhenKeyIsBlank()
    {
        var dto = new SettingsDto<SettingsValueV1Dto>
        {
            Key = "",
            Value = new SettingsValueV1Dto { StringValue = "anything" },
            ValueVersion = 1,
        };

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContractV1>();

        var exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(
            () => client.ManageSettingsAsync(dto));
        Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [TestMethod]
    public async Task DeleteSettings_RemovesMatchingSetting_WhenItExists()
    {
        var key = $"delete-{Guid.NewGuid()}";
        await SeedAsync(key, valueVersion: 1, schemaVersion: SchemaVersion, stringValue: "to-delete");

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContractV1>();

        await client.DeleteSettingsAsync(key, valueVersion: 1);

        var stored = await ReadPayloadAsync(key, valueVersion: 1, schemaVersion: SchemaVersion);
        Assert.IsNull(stored);
    }

    [TestMethod]
    public async Task DeleteSettings_Succeeds_WhenNothingMatches()
    {
        var key = $"delete-missing-{Guid.NewGuid()}";

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContractV1>();

        await client.DeleteSettingsAsync(key, valueVersion: 1);
    }

    [TestMethod]
    public async Task DeleteSettings_Throws400_WhenKeyIsBlank()
    {
        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContractV1>();

        var exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(
            () => client.DeleteSettingsAsync("", valueVersion: 1));
        Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    private static async Task SeedAsync(string key, int valueVersion, int schemaVersion, string stringValue)
    {
        var payload = JsonSerializer.Serialize(new SettingsValueV1Dto { StringValue = stringValue });

        await using var scope = MsSqlFixture.Factory.Services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();
        await repository.ManageSettingsAsync(payload, key, schemaVersion, valueVersion, CancellationToken.None);
    }

    private static async Task<string?> ReadPayloadAsync(string key, int valueVersion, int schemaVersion)
    {
        await using var scope = MsSqlFixture.Factory.Services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();
        return await repository.GetSettingsAsync(key, valueVersion, schemaVersion, CancellationToken.None);
    }

    private static async Task<SettingsValueV1Dto?> ReadValueAsync(string key, int valueVersion, int schemaVersion)
    {
        var payload = await ReadPayloadAsync(key, valueVersion, schemaVersion);
        return payload is null ? null : JsonSerializer.Deserialize<SettingsValueV1Dto>(payload);
    }
}
