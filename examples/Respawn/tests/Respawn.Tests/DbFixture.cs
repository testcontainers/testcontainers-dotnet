namespace Respawn.Tests;

[UsedImplicitly]
public sealed class DbFixture : IAsyncLifetime
{
    private readonly IContainer _postgreSqlContainer;

    public DbFixture()
    {
        // Testcontainers starts the dependent database (PostgreSQL) and copies the SQL scripts
        // to the container before it starts. The PostgreSQL container runs the scripts
        // automatically during startup, creating the database schema.
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithResourceMapping("migrate/", "/docker-entrypoint-initdb.d/")
            .Build();
    }

    public DbConnection DbConnection => new NpgsqlConnection(((PostgreSqlContainer)_postgreSqlContainer).GetConnectionString());

    public Task InitializeAsync()
    {
        return _postgreSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }
}