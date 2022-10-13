using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WeatherForecast.Entities;

namespace WeatherForecast.Repositories;

[PublicAPI]
public interface IWeatherDataReadOnlyRepository
{
  Task<IEnumerable<WeatherData>> GetAllAsync();

  Task<IEnumerable<WeatherData>> GetAllAsync(string latitude, string longitude, DateTime from, DateTime to);

  Task<WeatherData> GetAsync(Guid id);
}
