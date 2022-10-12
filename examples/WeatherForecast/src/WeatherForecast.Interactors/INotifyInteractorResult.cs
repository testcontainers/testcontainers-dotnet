using System;
using JetBrains.Annotations;

namespace WeatherForecast.Interactors;

[PublicAPI]
public interface INotifyInteractorResult<TEntity>
{
  event EventHandler<TEntity> Published;
}
