namespace WeatherForecast;

public sealed class DatabaseContainer : IHostedService
{
  private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:15.1").Build();

  public Task StartAsync(CancellationToken cancellationToken)
  {
    return _postgreSqlContainer.StartAsync(cancellationToken);
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return _postgreSqlContainer.StopAsync(cancellationToken);
  }

  public string GetConnectionString()
  {
    return _postgreSqlContainer.GetConnectionString();
  }
}
