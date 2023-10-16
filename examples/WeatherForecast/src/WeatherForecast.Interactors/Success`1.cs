namespace WeatherForecast.Interactors;

[PublicAPI]
public sealed class Success<TValue> : ResultInfo<TValue>
{
  public Success(TValue value)
    : base(StatusCode.Success, new Error(), value)
  {
  }
}
