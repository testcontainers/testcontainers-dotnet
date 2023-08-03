namespace Testcontainers.Kusto.Tests;

public sealed class KustoContainerTest : IAsyncLifetime
{
    private readonly KustoContainer _kustoContainer = new KustoBuilder().Build();

    public Task InitializeAsync()
    {
        return _kustoContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _kustoContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShowDatabaseReturnsDefaultDbInformation()
    {
        // Given
        using var client = KustoClientFactory.CreateCslAdminProvider(new KustoConnectionStringBuilder(_kustoContainer.GetConnectionString()));

        // When
        var dataReader = await client.ExecuteControlCommandAsync("NetDefaultDB", CslCommandGenerator.GenerateDatabaseShowCommand())
            .ConfigureAwait(false);

        dataReader.Read();

        // Then
        Assert.Equal("DatabaseName", dataReader.GetName(0));
        Assert.Equal("NetDefaultDB", dataReader.GetString(0));
    }
}