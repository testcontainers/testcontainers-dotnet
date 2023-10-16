namespace WeatherForecast.Interactors;

[PublicAPI]
public abstract class ResultInfo
{
  protected ResultInfo(StatusCode statusCode, Error error)
  {
    StatusCode = statusCode;
    Error = error;
    IsSuccessful = StatusCode.Success.Equals(StatusCode);
  }

  public StatusCode StatusCode { get; }

  public Error Error { get; }

  public bool IsSuccessful { get; }
}
