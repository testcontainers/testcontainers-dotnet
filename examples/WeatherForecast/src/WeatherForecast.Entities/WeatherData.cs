using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WeatherForecast.Entities;

[PublicAPI]
public readonly struct WeatherData
{
  public WeatherData(DateTime date, IEnumerable<Temperature> measurements)
  {
    IReadOnlyCollection<Temperature> temperatures = measurements.ToList();
    Date = date;
    Minimum = temperatures.OrderBy(temperature => temperature.Value).FirstOrDefault();
    Maximum = temperatures.OrderBy(temperature => temperature.Value).LastOrDefault();
    Temperatures = temperatures;
  }

  public DateTime Date { get; }

  public Temperature Minimum { get; }

  public Temperature Maximum { get; }

  public IEnumerable<Temperature> Temperatures { get; }
}
