namespace WeatherForecast.Repositories;

[PublicAPI]
public sealed class WeatherDataWriteOnlyRepository : IWeatherDataWriteOnlyRepository
{
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
