namespace Testcontainers.TUnit.Example4;

// The module test projects in this repository are written in xUnit.net and exercise the
// Testcontainers.Xunit base classes, in particular DbContainerFixture and the ADO.NET helper
// methods, as a side effect. Testcontainers.TUnit does not benefit from this coverage, so the
// tests below exercise DbContainerFixture and each helper method explicitly.

// # --8<-- [start:ConfigurePostgreSqlContainer]
[UsedImplicitly]
public sealed class PostgreSqlContainerFixture : DbContainerFixture<PostgreSqlBuilder, PostgreSqlContainer>
{
    public override DbProviderFactory DbProviderFactory
        => NpgsqlFactory.Instance;

    public override string ConnectionString
    {
        get
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(base.ConnectionString);
            connectionStringBuilder.Database = "chinook_auto_increment";
            return connectionStringBuilder.ConnectionString;
        }
    }

    protected override PostgreSqlBuilder Configure()
    {
        return new PostgreSqlBuilder("postgres:15.1")
            .WithResourceMapping("Chinook_PostgreSql_AutoIncrementPKs.sql", "/docker-entrypoint-initdb.d/");
    }
}
// # --8<-- [end:ConfigurePostgreSqlContainer]

// # --8<-- [start:InjectContainerFixture]
public sealed partial class PostgreSqlContainerTest
{
    [ClassDataSource<PostgreSqlContainerFixture>(Shared = SharedType.PerClass)]
    public required PostgreSqlContainerFixture Fixture { get; init; }
}
// # --8<-- [end:InjectContainerFixture]

public sealed partial class PostgreSqlContainerTest
{
    private const string SelectFirstAlbumTitle = "SELECT title FROM album WHERE album_id = 1";

    private const string SelectFirstArtistName = "SELECT name FROM artist WHERE artist_id = 1";

    private const string FirstAlbumTitle = "For Those About To Rock We Salute You";

    private const string FirstArtistName = "AC/DC";

    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ImageShouldMatchDefaultModuleImage()
    {
        await Assert.That(Fixture.Container.Image.FullName).IsEqualTo(PostgreSqlBuilder.PostgreSqlImage);
    }

    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateConnectionShouldReturnClosedConnection(CancellationToken cancellationToken)
    {
        await using var connection = Fixture.CreateConnection();
        await Assert.That(connection.State).IsEqualTo(ConnectionState.Closed);

        await connection.OpenAsync(cancellationToken);
        var title = await connection.QueryFirstAsync<string>(SelectFirstAlbumTitle);
        await Assert.That(title).IsEqualTo(FirstAlbumTitle);
    }

    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task OpenConnectionShouldReturnOpenConnection()
    {
        await using var connection = Fixture.OpenConnection();
        await Assert.That(connection.State).IsEqualTo(ConnectionState.Open);

        var title = await connection.QueryFirstAsync<string>(SelectFirstAlbumTitle);
        await Assert.That(title).IsEqualTo(FirstAlbumTitle);
    }

    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task OpenConnectionAsyncShouldReturnOpenConnection(CancellationToken cancellationToken)
    {
        await using var connection = await Fixture.OpenConnectionAsync(cancellationToken);
        await Assert.That(connection.State).IsEqualTo(ConnectionState.Open);

        var title = await connection.QueryFirstAsync<string>(SelectFirstAlbumTitle);
        await Assert.That(title).IsEqualTo(FirstAlbumTitle);
    }

    // # --8<-- [start:RunTests]
    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateCommandShouldExecuteAgainstDatabase(CancellationToken cancellationToken)
    {
        await using var command = Fixture.CreateCommand(SelectFirstAlbumTitle);
        var title = await command.ExecuteScalarAsync(cancellationToken);
        await Assert.That(title).IsEqualTo(FirstAlbumTitle);
    }
    // # --8<-- [end:RunTests]

    [Test]
    [Property(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateBatchShouldExecuteAgainstDatabase(CancellationToken cancellationToken)
    {
        await using var batch = Fixture.CreateBatch();

        var selectFirstAlbumTitle = batch.CreateBatchCommand();
        selectFirstAlbumTitle.CommandText = SelectFirstAlbumTitle;
        batch.BatchCommands.Add(selectFirstAlbumTitle);

        var selectFirstArtistName = batch.CreateBatchCommand();
        selectFirstArtistName.CommandText = SelectFirstArtistName;
        batch.BatchCommands.Add(selectFirstArtistName);

        await using var reader = await batch.ExecuteReaderAsync(cancellationToken);

        await Assert.That(await reader.ReadAsync(cancellationToken)).IsTrue();
        await Assert.That(reader.GetString(0)).IsEqualTo(FirstAlbumTitle);

        await Assert.That(await reader.NextResultAsync(cancellationToken)).IsTrue();
        await Assert.That(await reader.ReadAsync(cancellationToken)).IsTrue();
        await Assert.That(reader.GetString(0)).IsEqualTo(FirstArtistName);
    }
}