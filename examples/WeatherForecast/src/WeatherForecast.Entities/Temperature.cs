namespace WeatherForecast.Entities;

[PublicAPI]
public sealed class Temperature : HasId
{
  [JsonConstructor]
  public Temperature(Guid id, Guid belongsTo, string unitName, string unitSymbol, double value, DateTimeOffset measured) : base(id)
  {
    BelongsTo = belongsTo;
    UnitName = unitName;
    UnitSymbol = unitSymbol;
    Value = value;
    Measured = measured;
  }

  public static Temperature AbsoluteZero { get; } = Kelvin(Guid.Empty, 0, DateTimeOffset.MinValue);

  [JsonPropertyName("belongsTo")]
  public Guid BelongsTo { get; }

  [JsonPropertyName("unitName")]
  public string UnitName { get; }

  [JsonPropertyName("unitSymbol")]
  public string UnitSymbol { get; }

  [JsonPropertyName("value")]
  public double Value { get; }

  [JsonPropertyName("measured")]
  public DateTimeOffset Measured { get; }

  public static Temperature Kelvin(Guid belongsTo, double value, DateTimeOffset measured)
  {
    return new Temperature(Guid.NewGuid(), belongsTo, "Kelvin", "K", value, measured);
  }

  public static Temperature Celsius(Guid belongsTo, double value, DateTimeOffset measured)
  {
    return new Temperature(Guid.NewGuid(), belongsTo, "degree Celsius", "°C", value, measured);
  }

  public static Temperature Fahrenheit(Guid belongsTo, double value, DateTimeOffset measured)
  {
    return new Temperature(Guid.NewGuid(), belongsTo, "degree Fahrenheit", "°F", value, measured);
  }
}
