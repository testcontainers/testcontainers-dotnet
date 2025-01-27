namespace Testcontainers.Db2;

public sealed class Db2ContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseDb2Container]
    private readonly Db2Container _db2Container = new Db2Builder().Build();

    public Task InitializeAsync()
    {
        return _db2Container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _db2Container.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReadFromDb2Database()
    {
        // Given
        using DbConnection connection = new DB2Connection(_db2Container.GetConnectionString());

        // When
        connection.Open();

        using DbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";

        var actual = await command.ExecuteScalarAsync() as int?;
        Assert.Equal(1, actual.GetValueOrDefault());

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";

        // When
        var execResult = await _db2Container.ExecScriptAsync(scriptContent);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    // # --8<-- [end:UseDb2Container]
}