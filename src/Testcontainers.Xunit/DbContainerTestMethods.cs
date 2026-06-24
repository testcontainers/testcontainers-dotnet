namespace Testcontainers.Xunit;

internal sealed class DbContainerTestMethods(DbProviderFactory dbProviderFactory, Lazy<string> connectionString) : IDbContainerTestMethods, IAsyncDisposable
{
    private readonly DbProviderFactory _dbProviderFactory = dbProviderFactory ?? throw new ArgumentNullException(nameof(dbProviderFactory));
    private readonly Lazy<string> _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

#if NET8_0_OR_GREATER
    [CanBeNull]
    private DbDataSource _dbDataSource;
    private DbDataSource DbDataSource
    {
        get
        {
            _dbDataSource ??= _dbProviderFactory.CreateDataSource(_connectionString.Value);
            return _dbDataSource;
        }
    }

    public DbConnection CreateConnection() => DbDataSource.CreateConnection();

    public DbConnection OpenConnection() => DbDataSource.OpenConnection();

    public ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default) => DbDataSource.OpenConnectionAsync(cancellationToken);

    public DbCommand CreateCommand(string commandText = null) => DbDataSource.CreateCommand(commandText);

    public DbBatch CreateBatch() => DbDataSource.CreateBatch();

    public ValueTask DisposeAsync() => _dbDataSource?.DisposeAsync() ?? ValueTask.CompletedTask;
#else
    public DbConnection CreateConnection()
    {
        var connection = _dbProviderFactory.CreateConnection() ?? throw new InvalidOperationException($"DbProviderFactory.CreateConnection() returned null for {_dbProviderFactory}");
        connection.ConnectionString = _connectionString.Value;
        return connection;
    }

    public ValueTask DisposeAsync() => default;
#endif
}