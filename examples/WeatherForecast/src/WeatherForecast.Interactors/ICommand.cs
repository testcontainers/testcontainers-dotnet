using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WeatherForecast.Interactors;

[PublicAPI]
public interface ICommand<in TEntity>
{
  Task ExecuteAsync(TEntity arg);
}
