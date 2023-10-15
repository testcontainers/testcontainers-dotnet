namespace WeatherForecast.Contexts;

[PublicAPI]
public sealed class WeatherDataWriteOnlyContext : IWeatherDataWriteOnlyRepository
{
  public WeatherDataWriteOnlyContext(WeatherDataContext context)
  {
    _ = context;
  }

  public Task CreateAsync(WeatherData weatherData)
  {
    throw new NotImplementedException();
  }

  public Task UpdateAsync(WeatherData weatherData)
  {
    throw new NotImplementedException();
  }

  public Task DeleteAsync(WeatherData weatherData)
  {
    throw new NotImplementedException();
  }
}
