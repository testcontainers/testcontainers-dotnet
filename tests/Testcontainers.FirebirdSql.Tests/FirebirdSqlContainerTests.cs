namespace Testcontainers.FirebirdSql.Tests;

public abstract class FirebirdSqlContainerTests : IAsyncLifetime
{
    protected abstract FirebirdSqlContainer FirebirdSqlContainer { get; }

    public Task InitializeAsync()
    {
        return FirebirdSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return FirebirdSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new FbConnection(FirebirdSqlContainer.GetConnectionString());

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = FirebirdSqlContainer.TestQueryString;

        // When
        var execResult = await FirebirdSqlContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(false);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }
}

public class Firebird25SC : FirebirdSqlContainerTests
{
    protected override FirebirdSqlContainer FirebirdSqlContainer { get; } =
        new FirebirdSqlBuilder().WithImage("jacobalberty/firebird:2.5-sc").Build();
}

public class Firebird25SS : FirebirdSqlContainerTests
{
    protected override FirebirdSqlContainer FirebirdSqlContainer { get; } =
        new FirebirdSqlBuilder().WithImage("jacobalberty/firebird:2.5-ss").Build();
}

public class Firebird30 : FirebirdSqlContainerTests
{
    protected override FirebirdSqlContainer FirebirdSqlContainer { get; } =
        new FirebirdSqlBuilder().WithImage("jacobalberty/firebird:3.0").Build();
}

public class Firebird40 : FirebirdSqlContainerTests
{
    protected override FirebirdSqlContainer FirebirdSqlContainer { get; } =
        new FirebirdSqlBuilder().WithImage("jacobalberty/firebird:v4.0").Build();
}

public class FirebirdDefault : FirebirdSqlContainerTests
{
    protected override FirebirdSqlContainer FirebirdSqlContainer { get; } =
        new FirebirdSqlBuilder().Build();
}
