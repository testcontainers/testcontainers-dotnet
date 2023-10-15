namespace WeatherForecast.Interactors.SearchCityOrZipCode;

[PublicAPI]
public sealed class SearchCityOrZipCode : ISearchCityOrZipCode
{
  private readonly IWeatherDataReadOnlyRepository _readOnlyRepository;

  private readonly IWeatherDataWriteOnlyRepository _writeOnlyRepository;

  public SearchCityOrZipCode(IWeatherDataReadOnlyRepository readOnlyRepository, IWeatherDataWriteOnlyRepository writeOnlyRepository)
  {
    _readOnlyRepository = readOnlyRepository;
    _writeOnlyRepository = writeOnlyRepository;
  }

  public event EventHandler<ResultInfo<IEnumerable<WeatherData>>>? Published;

  public async Task ExecuteAsync(string cityOrZipCode)
  {
    ResultInfo<IEnumerable<WeatherData>> result;

    try
    {
      var weatherData = await _readOnlyRepository.GetAllAsync(string.Empty, string.Empty, DateTime.Today, DateTime.Today.AddDays(7))
        .ConfigureAwait(false);

      result = new Success<IEnumerable<WeatherData>>(weatherData);
    }
    catch
    {
      result = new Failure<IEnumerable<WeatherData>>(Array.Empty<WeatherData>());
    }

    Published?.Invoke(this, result);
  }
}
