namespace Testcontainers.Xunit.Example3;

// # --8<-- [start:ConfigurePostgreSqlContainer]
public sealed partial class PostgreSqlContainerTest(ITestOutputHelper testOutputHelper)
    : DbContainerTest<PostgreSqlBuilder, PostgreSqlContainer>(testOutputHelper)
{
    protected override PostgreSqlBuilder Configure(PostgreSqlBuilder builder)
    {
        return builder
            .WithImage("postgres:15.1")
            .WithResourceMapping("Chinook_PostgreSql_AutoIncrementPKs.sql", "/docker-entrypoint-initdb.d/");
    }
}
// # --8<-- [end:ConfigurePostgreSqlContainer]

public sealed partial class PostgreSqlContainerTest
{
    // # --8<-- [start:ConfigureDbProviderFactory]
    public override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;
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
    [Fact]
    public void ImageShouldMatchDefaultModuleImage()
    {
        Assert.Equal(PostgreSqlBuilder.PostgreSqlImage, Container.Image.FullName);
    }

    // # --8<-- [start:RunTests]
    [Fact]
    public async Task Test1()
    {
        const string sql = "SELECT title FROM album ORDER BY album_id";
        using var connection = await OpenConnectionAsync();
        var title = await connection.QueryFirstAsync<string>(sql);
        Assert.Equal("For Those About To Rock We Salute You", title);
    }
    // # --8<-- [end:RunTests]
}