namespace Testcontainers.Xunit;

/// <summary>
/// Base class for tests needing a database container per test method.
/// A logger is automatically configured to write messages to xUnit's <see cref="ITestOutputHelper"/>.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public abstract class DbContainerTest<TBuilderEntity, TContainerEntity> : ContainerTest<TBuilderEntity, TContainerEntity>, IDbContainerTestMethods
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer, IDatabaseContainer
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private DbContainerTestMethods _testMethods;

    protected DbContainerTest(ITestOutputHelper testOutputHelper, Func<TBuilderEntity, TBuilderEntity> configure = null)
        : base(testOutputHelper, configure)
    {
    }

    /// <inheritdoc />
    protected override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _testMethods = new DbContainerTestMethods(DbProviderFactory, ConnectionString);
    }

    /// <inheritdoc />
    protected override async Task DisposeAsync()
    {
        if (_testMethods != null)
        {
            await _testMethods.DisposeAsync()
                .ConfigureAwait(true);
        }

        await base.DisposeAsync()
            .ConfigureAwait(true);
    }

    /// <summary>
    /// The <see cref="DbProviderFactory"/> used to create <see cref="DbConnection"/> instances.
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