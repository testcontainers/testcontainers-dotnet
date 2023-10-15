namespace WeatherForecast.Interactors;

[PublicAPI]
public sealed class Failure<TValue> : ResultInfo<TValue>
{
  public Failure(TValue value)
    : base(StatusCode.Failure, new Error(), value)
  {
  }
}
