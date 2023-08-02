namespace Testcontainers.Kusto.Tests;

public class KustoContainerTest : IAsyncLifetime
{
    private readonly KustoContainer _kustainer = new KustoBuilder().Build();

    public Task InitializeAsync()
    {
        return _kustainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _kustainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShowDatabaseReturnsDefaultDbInformation()
    {
        // Given
        using var client = KustoClientFactory.CreateCslAdminProvider(new KustoConnectionStringBuilder(_kustainer.GetConnectionString()));

        // When
        var dataReader = await client.ExecuteControlCommandAsync("NetDefaultDB", CslCommandGenerator.GenerateDatabaseShowCommand());
        dataReader.Read();

        // Then
        Assert.Equal("DatabaseName", dataReader.GetName(0));
        Assert.Equal("NetDefaultDB", dataReader.GetString(0));
    }
}
