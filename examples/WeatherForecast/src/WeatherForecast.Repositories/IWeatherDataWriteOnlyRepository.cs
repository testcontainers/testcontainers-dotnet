using System.Threading.Tasks;
using JetBrains.Annotations;
using WeatherForecast.Entities;

namespace WeatherForecast.Repositories;

[PublicAPI]
public interface IWeatherDataWriteOnlyRepository
{
  Task CreateAsync(WeatherData weatherData);

  Task UpdateAsync(WeatherData weatherData);

  Task DeleteAsync(WeatherData weatherData);
}
