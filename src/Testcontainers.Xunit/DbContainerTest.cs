namespace Testcontainers.Xunit;

/// <summary>
/// Base class for tests needing a database container per test method.
/// A logger is automatically configured to write messages to xUnit's <see cref="ITestOutputHelper" />.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public abstract class DbContainerTest<TBuilderEntity, TContainerEntity>(ITestOutputHelper testOutputHelper, Func<TBuilderEntity, TBuilderEntity> configure = null)
    : ContainerTest<TBuilderEntity, TContainerEntity>(testOutputHelper, configure), IDbContainerTestMethods
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer, IDatabaseContainer
{
    private DbContainerTestMethods _testMethods;

    /// <inheritdoc />
    protected override async LifetimeTask InitializeAsync()
    {
        await base.InitializeAsync()
            .ConfigureAwait(false);

        _testMethods = new DbContainerTestMethods(DbProviderFactory, new Lazy<string>(() => ConnectionString));
    }

    /// <inheritdoc />
    protected override async LifetimeTask DisposeAsyncCore()
    {
        if (_testMethods != null)
        {
            await _testMethods.DisposeAsync()
                .ConfigureAwait(true);
        }

        await base.DisposeAsyncCore()
            .ConfigureAwait(true);
    }

    /// <summary>
    /// The <see cref="DbProviderFactory" /> used to create <see cref="DbConnection" /> instances.
    /// </summary>
    public abstract DbProviderFactory DbProviderFactory { get; }

    /// <summary>
    /// Gets the database connection string.
    /// </summary>
    public virtual string ConnectionString => Container.GetConnectionString();

    /// <inheritdoc />
    public DbConnection CreateConnection() => _testMethods.CreateConnection();

#if NET8_0_OR_GREATER
    /// <inheritdoc />
    public DbConnection OpenConnection() => _testMethods.OpenConnection();

    /// <inheritdoc />
    public ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default) => _testMethods.OpenConnectionAsync(cancellationToken);

    /// <inheritdoc />
    public DbCommand CreateCommand(string commandText = null) => _testMethods.CreateCommand(commandText);

    /// <inheritdoc />
    public DbBatch CreateBatch() => _testMethods.CreateBatch();
#endif
}