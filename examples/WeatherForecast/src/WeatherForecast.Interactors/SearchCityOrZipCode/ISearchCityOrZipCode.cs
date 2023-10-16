namespace WeatherForecast.Interactors.SearchCityOrZipCode;

[PublicAPI]
public interface ISearchCityOrZipCode : ICommand<string>, INotifyInteractorResult<ResultInfo<IEnumerable<WeatherData>>>
{
}
