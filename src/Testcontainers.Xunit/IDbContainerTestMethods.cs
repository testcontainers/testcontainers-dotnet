namespace Testcontainers.Xunit;

/// <summary>
/// Method to ease working with DbConnection, DbCommand and DbBatch provided by both
/// <see cref="DbContainerFixture{TBuilderEntity,TContainerEntity}" /> and <see cref="DbContainerTest{TBuilderEntity,TContainerEntity}" />.
/// </summary>
internal interface IDbContainerTestMethods
{
    /// <summary>
    /// Returns a new, closed connection to the database.
    /// </summary>
    /// <remarks>
    /// The connection must be opened before it can be used.
    /// <para />
    /// It is the responsibility of the caller to properly dispose the connection returned by this method. Failure to do so may result in a connection leak.
    /// </remarks>
    /// <returns>A new, closed connection to the database.</returns>
    DbConnection CreateConnection();

#if NET8_0_OR_GREATER
    /// <summary>
    /// Returns a new, open connection to the database.
    /// </summary>
    /// <remarks>
    /// The returned connection is already open, and is ready for immediate use.
    /// <para />
    /// It is the responsibility of the caller to properly dispose the connection returned by this method. Failure to do so may result in a connection leak.
    /// </remarks>
    /// <returns>A new, open connection to the database.</returns>
    DbConnection OpenConnection();

    /// <summary>
    /// Asynchronously returns a new, open connection to the database.
    /// </summary>
    /// <remarks>
    /// The returned connection is already open, and is ready for immediate use.
    /// <para />
    /// It is the responsibility of the caller to properly dispose the connection returned by this method. Failure to do so may result in a connection leak.
    /// </remarks>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A new, open connection to the database.</returns>
    ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a <see cref="DbCommand" /> that's ready for execution against the database.
    /// </summary>
    /// <remarks>
    /// Commands returned from this method are already configured to execute against the database; their <see cref="DbCommand.Connection" /> does not need to be set, and doing so will throw an exception.
    /// </remarks>
    /// <param name="commandText">The text command with which to initialize the <see cref="DbCommand" /> that this method returns.</param>
    /// <returns>A <see cref="DbCommand" /> that's ready for execution against the database.</returns>
    DbCommand CreateCommand([CanBeNull] string commandText = null);

    /// <summary>
    /// Returns a <see cref="DbBatch" /> that's ready for execution against the database.
    /// </summary>
    /// <remarks>
    /// Batches returned from this method are already configured to execute against the database; their <see cref="DbCommand.Connection" /> does not need to be set, and doing so will throw an exception.
    /// </remarks>
    /// <returns>A <see cref="DbBatch" /> that's ready for execution against the database.</returns>
    DbBatch CreateBatch();
#endif
}