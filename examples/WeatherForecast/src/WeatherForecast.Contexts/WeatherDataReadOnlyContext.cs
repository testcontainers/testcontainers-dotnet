namespace WeatherForecast.Contexts;

[PublicAPI]
public sealed class WeatherDataReadOnlyContext : IWeatherDataReadOnlyRepository
{
  private readonly WeatherDataContext _context;

  public WeatherDataReadOnlyContext(WeatherDataContext context)
  {
    _context = context;
    _context.Database.EnsureCreated();
  }

  public Task<IEnumerable<WeatherData>> GetAllAsync()
  {
    throw new NotImplementedException();
  }

  public Task<IEnumerable<WeatherData>> GetAllAsync(string latitude, string longitude, DateTime from, DateTime to)
  {
    return Task.FromResult<IEnumerable<WeatherData>>(_context.WeatherData.Include(property => property.Temperatures).OrderBy(weatherData => weatherData.Date).Take(to.Subtract(from).Days));
  }

  public Task<WeatherData> GetAsync(Guid id)
  {
    throw new NotImplementedException();
  }
}
