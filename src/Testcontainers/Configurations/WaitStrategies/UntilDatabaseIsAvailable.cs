namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Data.Common;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilDatabaseIsAvailable : IWaitUntil
  {
    private readonly DbProviderFactory _dbProviderFactory;

    public UntilDatabaseIsAvailable(DbProviderFactory dbProviderFactory)
    {
      _dbProviderFactory = dbProviderFactory;
    }

    public async Task<bool> UntilAsync(IContainer container)
    {
      if (container is not IDatabaseContainer dbContainer)
      {
        throw new NotSupportedException(
          $"The 'UntilDatabaseIsAvailable' wait strategy can only be used with database containers. "
            + $"The provided container type '{container.GetType().FullName}' does not implement '{nameof(IDatabaseContainer)}'."
        );
      }

      var connection = _dbProviderFactory.CreateConnection();
      if (connection == null)
      {
        throw new InvalidOperationException(
          $"Failed to create a database connection. The factory '{_dbProviderFactory.GetType().FullName}' returned null from 'CreateConnection()'."
        );
      }

      try
      {
        connection.ConnectionString = dbContainer.GetConnectionString();

        await connection.OpenAsync().ConfigureAwait(false);

        return true;
      }
      catch
      {
        return false;
      }
      finally
      {
        connection.Dispose();
      }
    }
  }
}
