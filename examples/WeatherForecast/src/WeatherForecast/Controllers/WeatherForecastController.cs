namespace WeatherForecast.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class WeatherForecastController : ControllerBase, IDisposable
{
  private readonly ISearchCityOrZipCode _searchCityOrZipCode;

  private readonly EventWaitHandle _wait = new AutoResetEvent(false);

  private IEnumerable<WeatherData> _weatherData = Array.Empty<WeatherData>();

  public WeatherForecastController(ISearchCityOrZipCode searchCityOrZipCode)
  {
    _searchCityOrZipCode = searchCityOrZipCode;
    _searchCityOrZipCode.Published += SearchCityOrZipCodeResultPublished;
  }

  public void Dispose()
  {
    _searchCityOrZipCode.Published -= SearchCityOrZipCodeResultPublished;

    _wait.Dispose();
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<WeatherData>>> GetWeatherForecast()
  {
    await _searchCityOrZipCode.ExecuteAsync(string.Empty)
      .ConfigureAwait(false);

    _wait.WaitOne();

    return Ok(_weatherData);
  }

  private void SearchCityOrZipCodeResultPublished(object? sender, ResultInfo<IEnumerable<WeatherData>> result)
  {
    if (result.IsSuccessful)
    {
      _weatherData = result.Value;
    }

    _wait.Set();
  }
}
