namespace Testcontainers.TUnit;

/// <summary>
/// Implements the ADO.NET helper methods shared by <see cref="DbContainerFixture{TBuilderEntity,TContainerEntity}" /> and <see cref="DbContainerTest{TBuilderEntity,TContainerEntity}" />.
/// </summary>
internal sealed class DbContainerTestMethods : IDbContainerTestMethods, IAsyncDisposable
{
    private readonly DbProviderFactory _dbProviderFactory;

    private readonly Lazy<string> _connectionString;

#if NET8_0_OR_GREATER
    // TUnit runs the tests that share a fixture in parallel, hence the data source is created lazily and thread-safe.
    private readonly Lazy<DbDataSource> _dbDataSource;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="DbContainerTestMethods" /> class.
    /// </summary>
    /// <param name="dbProviderFactory">The <see cref="DbProviderFactory" /> used to create <see cref="DbConnection" /> instances.</param>
    /// <param name="connectionString">The database connection string, resolved on first use.</param>
    public DbContainerTestMethods(DbProviderFactory dbProviderFactory, Lazy<string> connectionString)
    {
        _dbProviderFactory = dbProviderFactory ?? throw new ArgumentNullException(nameof(dbProviderFactory));
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
#if NET8_0_OR_GREATER
        _dbDataSource = new Lazy<DbDataSource>(() => _dbProviderFactory.CreateDataSource(_connectionString.Value));
#endif
    }

#if NET8_0_OR_GREATER
    /// <inheritdoc />
    public DbConnection CreateConnection() => _dbDataSource.Value.CreateConnection();

    /// <inheritdoc />
    public DbConnection OpenConnection() => _dbDataSource.Value.OpenConnection();

    /// <inheritdoc />
    public ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default) => _dbDataSource.Value.OpenConnectionAsync(cancellationToken);

    /// <inheritdoc />
    public DbCommand CreateCommand(string commandText = null) => _dbDataSource.Value.CreateCommand(commandText);

    /// <inheritdoc />
    public DbBatch CreateBatch() => _dbDataSource.Value.CreateBatch();

    /// <inheritdoc />
    public ValueTask DisposeAsync() => _dbDataSource.IsValueCreated ? _dbDataSource.Value.DisposeAsync() : ValueTask.CompletedTask;
#else
    /// <inheritdoc />
    public DbConnection CreateConnection()
    {
        var connection = _dbProviderFactory.CreateConnection() ?? throw new InvalidOperationException($"DbProviderFactory.CreateConnection() returned null for {_dbProviderFactory}");
        connection.ConnectionString = _connectionString.Value;
        return connection;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => default;
#endif
}