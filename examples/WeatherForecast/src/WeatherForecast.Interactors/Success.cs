namespace WeatherForecast.Interactors;

[PublicAPI]
public sealed class Success : ResultInfo
{
  public Success()
    : base(StatusCode.Success, new Error())
  {
  }
}
