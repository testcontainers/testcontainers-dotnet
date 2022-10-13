using System.Collections.Generic;
using JetBrains.Annotations;
using WeatherForecast.Entities;

namespace WeatherForecast.Interactors.SearchCityOrZipCode;

[PublicAPI]
public interface ISearchCityOrZipCode : ICommand<string>, INotifyInteractorResult<ResultInfo<IEnumerable<WeatherData>>>
{
}
