namespace Testcontainers.TUnit.Example3;

// # --8<-- [start:ConfigurePostgreSqlContainer]
public sealed partial class PostgreSqlContainerTest : DbContainerTest<PostgreSqlBuilder, PostgreSqlContainer>
{
    protected override PostgreSqlBuilder Configure()
    {
        return new PostgreSqlBuilder("postgres:15.1")
            .WithResourceMapping("Chinook_PostgreSql_AutoIncrementPKs.sql", "/docker-entrypoint-initdb.d/");
    }
}
// # --8<-- [end:ConfigurePostgreSqlContainer]

public sealed partial class PostgreSqlContainerTest
{
    // # --8<-- [start:ConfigureDbProviderFactory]
    public override DbProviderFactory DbProviderFactory
        => NpgsqlFactory.Instance;
    // # --8<-- [end:ConfigureDbProviderFactory]
}

public sealed partial class PostgreSqlContainerTest
{
    public override string ConnectionString
    {
        get
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(base.ConnectionString);
            connectionStringBuilder.Database = "chinook_auto_increment";
            return connectionStringBuilder.ConnectionString;
        }
    }
}

public sealed partial class PostgreSqlContainerTest
{
    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ImageShouldMatchDefaultModuleImage()
    {
        await Assert.That(Container.Image.FullName).IsEqualTo(PostgreSqlBuilder.PostgreSqlImage);
    }

    // # --8<-- [start:RunTests]
    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task Test1(CancellationToken cancellationToken)
    {
        const string sql = "SELECT title FROM album ORDER BY album_id";
        using var connection = await OpenConnectionAsync(cancellationToken);
        var title = await connection.QueryFirstAsync<string>(sql);
        await Assert.That(title).IsEqualTo("For Those About To Rock We Salute You");
    }
    // # --8<-- [end:RunTests]

    // Exercises the remaining ADO.NET helper methods of DbContainerTest. The shared fixture
    // variant is covered by the tests in PostgreSqlContainerFixture.cs.
    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HelperMethodsShouldBeBoundToDatabase(CancellationToken cancellationToken)
    {
        const string sql = "SELECT title FROM album WHERE album_id = 1";
        const string expectedTitle = "For Those About To Rock We Salute You";

        await using var closedConnection = CreateConnection();
        await Assert.That(closedConnection.State).IsEqualTo(ConnectionState.Closed);

        await using var openConnection = OpenConnection();
        await Assert.That(openConnection.State).IsEqualTo(ConnectionState.Open);

        await using var command = CreateCommand(sql);
        await Assert.That(await command.ExecuteScalarAsync(cancellationToken)).IsEqualTo(expectedTitle);

        await using var batch = CreateBatch();
        var batchCommand = batch.CreateBatchCommand();
        batchCommand.CommandText = sql;
        batch.BatchCommands.Add(batchCommand);
        await Assert.That(await batch.ExecuteScalarAsync(cancellationToken)).IsEqualTo(expectedTitle);
    }
}