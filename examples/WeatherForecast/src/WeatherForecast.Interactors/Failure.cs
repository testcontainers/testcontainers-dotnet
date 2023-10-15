namespace WeatherForecast.Interactors;

[PublicAPI]
public sealed class Failure : ResultInfo
{
  public Failure()
    : base(StatusCode.Failure, new Error())
  {
  }
}
