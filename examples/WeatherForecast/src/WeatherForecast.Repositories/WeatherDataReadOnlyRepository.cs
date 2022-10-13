using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WeatherForecast.Entities;

namespace WeatherForecast.Repositories;

[PublicAPI]
public sealed class WeatherDataReadOnlyRepository : IWeatherDataReadOnlyRepository
{
  private static readonly ThreadLocal<Random> Random = new(() => new Random());

  public Task<IEnumerable<WeatherData>> GetAllAsync()
  {
    throw new NotImplementedException();
  }

  public Task<IEnumerable<WeatherData>> GetAllAsync(string latitude, string longitude, DateTime from, DateTime to)
  {
    return Task.FromResult(Enumerable.Range(0, to.Subtract(from).Days).Select(day => DateTime.Today.AddDays(day)).Select(day => new WeatherData(day, Enumerable.Range(0, 12).Select(hour => Temperature.Celsius(Random.Value.Next(-10, 30), DateTime.Today.AddHours(hour))))));
  }

  public Task<WeatherData> GetAsync(Guid id)
  {
    throw new NotImplementedException();
  }
}
