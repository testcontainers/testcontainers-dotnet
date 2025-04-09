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
        throw new NotSupportedException($"The \"UntilDatabaseIsAvailable\" wait strategy is only available on database containers. " +
                                        $"{container.GetType().FullName} does not implement the {nameof(IDatabaseContainer)} interface.");
      }

      using (var connection = _dbProviderFactory.CreateConnection() ?? throw new InvalidOperationException($"{_dbProviderFactory.GetType().FullName}.CreateConnection() returned null."))
      {
        connection.ConnectionString = dbContainer.GetConnectionString();
        try
        {
          await connection.OpenAsync();
          return true;
        }
        catch
        {
          return false;
        }
      }
    }
  }
}
