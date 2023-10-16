namespace WeatherForecast.Entities;

[PublicAPI]
public sealed class WeatherData : HasId
{
  public WeatherData(Guid id, DateTime date) : this(id, date, new List<Temperature>())
  {
    // Entity Framework constructor.
  }

  [JsonConstructor]
  public WeatherData(Guid id, DateTime date, IList<Temperature> temperatures) : base(id)
  {
    Date = date;
    Minimum = temperatures.OrderBy(temperature => temperature.Value).DefaultIfEmpty(Temperature.AbsoluteZero).First();
    Maximum = temperatures.OrderBy(temperature => temperature.Value).DefaultIfEmpty(Temperature.AbsoluteZero).Last();
    Temperatures = temperatures;
  }

  [JsonPropertyName("date")]
  public DateTime Date { get; }

  [JsonIgnore]
  public Temperature Minimum { get; }

  [JsonIgnore]
  public Temperature Maximum { get; }

  [JsonPropertyName("temperatures")]
  public IList<Temperature> Temperatures { get; }
}
