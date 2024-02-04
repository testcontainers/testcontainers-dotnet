namespace WeatherForecast.Repositories;

[PublicAPI]
public sealed class WeatherDataReadOnlyRepository : IWeatherDataReadOnlyRepository
{
  public Task<IEnumerable<WeatherData>> GetAllAsync()
  {
    throw new NotImplementedException();
  }

  public Task<IEnumerable<WeatherData>> GetAllAsync(string latitude, string longitude, DateTime from, DateTime to)
  {
    return Task.FromResult(Enumerable.Range(0, to.Subtract(from).Days).Select(_ => Guid.NewGuid()).Select((id, day) => new WeatherData(id, DateTime.Today.AddDays(day), Enumerable.Range(0, 23).Select(hour => Temperature.Celsius(id, StaticRandom.Next(-10, 30), DateTime.Today.AddDays(day).AddHours(hour))).ToList())));
  }

  public Task<WeatherData> GetAsync(Guid id)
  {
    throw new NotImplementedException();
  }

  private static class StaticRandom
  {
    private static readonly Random Random = new Random();

    private static readonly object MyLock = new object();

    public static int Next(int min, int max)
    {
      lock (MyLock)
      {
        return Random.Next(min, max);
      }
    }
  }
}
