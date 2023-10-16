namespace WeatherForecast.Interactors;

[PublicAPI]
public sealed class Error
{
  public string LocalizedDescription { get; }
    = string.Empty;
}
