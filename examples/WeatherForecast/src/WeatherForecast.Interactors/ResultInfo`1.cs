namespace WeatherForecast.Interactors;

[PublicAPI]
public abstract class ResultInfo<TValue> : ResultInfo
{
  protected ResultInfo(StatusCode statusCode, Error error, TValue value)
    : base(statusCode, error)
  {
    Value = value;
  }

  public TValue Value { get; }
}
