namespace WeatherForecast.Interactors;

[PublicAPI]
public interface INotifyInteractorResult<TEntity>
{
  event EventHandler<TEntity> Published;
}
