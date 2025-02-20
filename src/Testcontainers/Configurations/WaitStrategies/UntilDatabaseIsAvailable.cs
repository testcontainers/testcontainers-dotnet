namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Data.Common;
  using System.Diagnostics;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal class UntilDatabaseIsAvailable : IWaitUntil
  {
    private static readonly TimeSpan DefaultFrequency = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(5);

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly TimeSpan _frequency;
    private readonly TimeSpan _timeout;

    public UntilDatabaseIsAvailable(DbProviderFactory dbProviderFactory, TimeSpan frequency, TimeSpan timeout)
    {
      _dbProviderFactory = dbProviderFactory;
      _frequency = frequency == TimeSpan.Zero ? DefaultFrequency : frequency;
      _timeout = timeout == TimeSpan.Zero ? DefaultTimeout : timeout;
    }

    public async Task<bool> UntilAsync(IContainer container)
    {
      if (container is not IDatabaseContainer dbContainer)
      {
        throw new NotSupportedException($"The \"UntilDatabaseIsAvailable\" wait strategy is only available on database containers. " +
                                        $"{container.GetType().FullName} does not implement the {nameof(IDatabaseContainer)} interface.");
      }

      var stopwatch = Stopwatch.StartNew();

      var connectionString = dbContainer.GetConnectionString();
      while (!await IsAvailableAsync(connectionString, stopwatch))
      {
        await Task.Delay(_frequency);
      }

      return true;
    }

    private async Task<bool> IsAvailableAsync(string connectionString, Stopwatch stopwatch)
    {
      using (var connection = _dbProviderFactory.CreateConnection() ?? throw new InvalidOperationException($"{_dbProviderFactory.GetType().FullName}.CreateConnection() returned null."))
      {
        connection.ConnectionString = connectionString;
        try
        {
          await connection.OpenAsync();
          return true;
        }
        catch (Exception exception)
        {
          if (stopwatch.Elapsed > _timeout)
          {
            throw new TimeoutException($"The database was not available at \"{connectionString}\" after waiting for {_timeout.TotalSeconds:F0} seconds.", exception);
          }
          return false;
        }
      }
    }
  }
}
