using System.Net;
using Api.Abstractions;
using Api.Abstractions.Dtos;
using Core.Abstractions.Domains;
using Infrastructure.Database.Repositories.Settings;
using Microsoft.Extensions.DependencyInjection;
using Tests.Infrastructure;

namespace Tests.Controllers.V1;

[TestClass]
public sealed class SettingsControllerTests
{
    [TestMethod]
    public async Task GetSettings_ReturnsMatchingRow_WhenSettingExistsForKeyAndValueVersion()
    {
        var key = $"test-{Guid.NewGuid()}";
        await SeedAsync(key, valueVersion: 1, schemaVersion: 1, value: "first");
        await SeedAsync(key, valueVersion: 1, schemaVersion: 2, value: "second");

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContract>();

        var dto = await client.GetSettingsAsync(key, valueVersion: 1);

        var itemDto = Assert.ContainsSingle(dto.Items);
        
        Assert.AreEqual(key, itemDto.Key);
        Assert.AreEqual("first", itemDto.Value);
        Assert.AreEqual(1, itemDto.SchemaVersion);
        Assert.AreEqual(1, itemDto.ValueVersion);
    }

    [TestMethod]
    public async Task GetSettings_Throws_WhenNoSettingsMatch()
    {
        var key = $"missing-{Guid.NewGuid()}";

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContract>();

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => client.GetSettingsAsync(key, valueVersion: 1));
    }

    [TestMethod]
    public async Task GetSettings_Throws400_WhenKeyIsBlank()
    {
        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContract>();

        var exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(
            () => client.GetSettingsAsync("", valueVersion: 1));
        Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [TestMethod]
    public async Task ManageSettings_CreatesNewSetting_WhenCompositeKeyDoesNotExist()
    {
        var key = $"manage-create-{Guid.NewGuid()}";
        var dto = new SettingsItemDto
        {
            Key = key,
            Value = "created-via-client",
            SchemaVersion = 1,
            ValueVersion = 1,
        };

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContract>();

        await client.ManageSettingsAsync(dto);

        var fetched = await client.GetSettingsAsync(key, valueVersion: 1);
        var item = Assert.ContainsSingle(fetched.Items);
        Assert.AreEqual("created-via-client", item.Value);
    }

    [TestMethod]
    public async Task ManageSettings_UpdatesExistingSetting_WhenCompositeKeyMatches()
    {
        var key = $"manage-update-{Guid.NewGuid()}";
        await SeedAsync(key, valueVersion: 1, schemaVersion: 1, value: "original");

        var dto = new SettingsItemDto
        {
            Key = key,
            Value = "updated",
            SchemaVersion = 1,
            ValueVersion = 1,
        };

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContract>();

        await client.ManageSettingsAsync(dto);

        var fetched = await client.GetSettingsAsync(key, valueVersion: 1);
        var item = Assert.ContainsSingle(fetched.Items);
        Assert.AreEqual("updated", item.Value);
    }

    [TestMethod]
    public async Task ManageSettings_Throws400_WhenKeyIsBlank()
    {
        var dto = new SettingsItemDto
        {
            Key = "",
            Value = "anything",
            SchemaVersion = 1,
            ValueVersion = 1,
        };

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContract>();

        var exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(
            () => client.ManageSettingsAsync(dto));
        Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [TestMethod]
    public async Task DeleteSettings_RemovesMatchingSetting_WhenItExists()
    {
        var key = $"delete-{Guid.NewGuid()}";
        await SeedAsync(key, valueVersion: 1, schemaVersion: 1, value: "to-delete");

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContract>();

        await client.DeleteSettingsAsync(key, valueVersion: 1);

        var stored = await ReadAsync(key, valueVersion: 1, schemaVersion: 1);
        Assert.IsEmpty(stored.Items);
    }

    [TestMethod]
    public async Task DeleteSettings_Succeeds_WhenNothingMatches()
    {
        var key = $"delete-missing-{Guid.NewGuid()}";

        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContract>();

        await client.DeleteSettingsAsync(key, valueVersion: 1);
    }

    [TestMethod]
    public async Task DeleteSettings_Throws400_WhenKeyIsBlank()
    {
        using var scope = MsSqlFixture.Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISettingsContract>();

        var exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(
            () => client.DeleteSettingsAsync("", valueVersion: 1));
        Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    private static async Task SeedAsync(string key, int valueVersion, int schemaVersion, string value)
    {
        await using var scope = MsSqlFixture.Factory.Services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();
        await repository.ManageSettingsAsync(
            new SettingsItemDomain
            {
                Key = key,
                Value = value,
                ValueVersion = valueVersion,
                SchemaVersion = schemaVersion,
            },
            CancellationToken.None);
    }

    private static async Task<SettingsDomain> ReadAsync(string key, int valueVersion, int schemaVersion)
    {
        await using var scope = MsSqlFixture.Factory.Services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();
        return await repository.GetSettingsAsync(key, valueVersion, schemaVersion, CancellationToken.None);
    }
}
