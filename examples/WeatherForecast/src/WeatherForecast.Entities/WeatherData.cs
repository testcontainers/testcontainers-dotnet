namespace WeatherForecast.Entities;

[PublicAPI]
public sealed class WeatherData : HasId
{
  public WeatherData(Guid id, DateTimeOffset period) : this(id, period, new List<Temperature>())
  {
    // Entity Framework constructor.
  }

  [JsonConstructor]
  public WeatherData(Guid id, DateTimeOffset period, IList<Temperature> temperatures) : base(id)
  {
    Period = period;
    Minimum = temperatures.OrderBy(temperature => temperature.Value).DefaultIfEmpty(Temperature.AbsoluteZero).First();
    Maximum = temperatures.OrderBy(temperature => temperature.Value).DefaultIfEmpty(Temperature.AbsoluteZero).Last();
    Temperatures = temperatures;
  }

  [JsonPropertyName("period")]
  public DateTimeOffset Period { get; }

  [JsonIgnore]
  public Temperature Minimum { get; }

  [JsonIgnore]
  public Temperature Maximum { get; }

  [JsonPropertyName("temperatures")]
  public IList<Temperature> Temperatures { get; }
}
