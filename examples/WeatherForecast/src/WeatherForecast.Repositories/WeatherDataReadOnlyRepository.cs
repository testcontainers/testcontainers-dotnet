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
    return Task.FromResult(Enumerable.Range(0, to.Subtract(from).Days).Select(_ => Guid.NewGuid()).Select((id, day) => new WeatherData(id, DateTime.Today.AddDays(day), Enumerable.Range(0, 23).Select(hour => Temperature.Celsius(id, Random.Value.Next(-10, 30), DateTime.Today.AddDays(day).AddHours(hour))).ToList())));
  }

  public Task<WeatherData> GetAsync(Guid id)
  {
    throw new NotImplementedException();
  }
}
