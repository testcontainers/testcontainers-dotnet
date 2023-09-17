namespace Flyway.Tests;

[UsedImplicitly]
public sealed class DbFixture : IAsyncLifetime
{
    private readonly INetwork _network = new NetworkBuilder().Build();

    private readonly IContainer _postgreSqlContainer;

    private readonly IContainer _flywayContainer;

    public DbFixture()
    {
        // Testcontainers starts the dependent database (PostgreSQL) and the database
        // migration tool Flyway. It establishes a network connection between these two
        // containers. Before starting the Flyway container, Testcontainers copies the SQL
        // migration files into it. When the Flyway container starts, it initiates the
        // dependent database container, connects to it, and begins the database migration
        // as soon as the database is ready. Once the migration is finished, the Flyway
        // container exits, and the database container becomes available for tests.

        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithNetwork(_network)
            .WithNetworkAliases(nameof(_postgreSqlContainer))
            .Build();

        // The member `WithResourceMapping(string, string)` copies the SQL migration files
        // from the test host into the Flyway container before it starts. This ensures that
        // the files are available as soon as the container starts. Flyway will
        // automatically pick them up and start the database migration process.

        _flywayContainer = new ContainerBuilder()
            .WithImage("flyway/flyway:9-alpine")
            .WithResourceMapping("migrate/", "/flyway/sql/")
            .WithCommand("-url=jdbc:postgresql://" + nameof(_postgreSqlContainer) + "/")
            .WithCommand("-user=" + PostgreSqlBuilder.DefaultUsername)
            .WithCommand("-password=" + PostgreSqlBuilder.DefaultPassword)
            .WithCommand("-connectRetries=3")
            .WithCommand("migrate")
            .WithNetwork(_network)
            .DependsOn(_postgreSqlContainer)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new MigrationCompleted()))
            .Build();
    }

    public DbConnection DbConnection => new NpgsqlConnection(((PostgreSqlContainer)_postgreSqlContainer).GetConnectionString());

    public Task InitializeAsync()
    {
        return _flywayContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        // We do not need to manually dispose Docker resources. If resources depend on each
        // other, it is necessary to dispose them in the correct order. Testcontainers'
        // Resource Reaper (Ryuk) will reliably take care of these resources and dispose
        // them after the test automatically.
        return Task.CompletedTask;
    }

    private sealed class MigrationCompleted : IWaitUntil
    {
        // The Flyway container will exit after executing the database migration. We do not
        // check if the migration was successful. To verify its success, we can either
        // check the exit code of the container or the console output, respectively the
        // standard output (stdout) or error output (stderr).
        public Task<bool> UntilAsync(IContainer container)
        {
            return Task.FromResult(TestcontainersStates.Exited.Equals(container.State));
        }
    }
}