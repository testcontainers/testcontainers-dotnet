namespace Testcontainers.Kusto;

public sealed class KustoContainerTest : IAsyncLifetime
{
    private readonly KustoContainer _kustoContainer = new KustoBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _kustoContainer.StartAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _kustoContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShowDatabaseReturnsDefaultDbInformation()
    {
        // Given
        using var client = KustoClientFactory.CreateCslAdminProvider(
            _kustoContainer.GetConnectionString()
        );

        // When
        using var dataReader = await client
            .ExecuteControlCommandAsync(
                "NetDefaultDB",
                CslCommandGenerator.GenerateDatabaseShowCommand()
            )
            .ConfigureAwait(true);

        _ = dataReader.Read();

        // Then
        Assert.Equal("DatabaseName", dataReader.GetName(0));
        Assert.Equal("NetDefaultDB", dataReader.GetString(0));
    }
}
