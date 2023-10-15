namespace WeatherForecast.Entities;

[PublicAPI]
public abstract class HasId
{
  [JsonConstructor]
  public HasId(Guid id)
  {
    Id = id;
  }

  [Key]
  [JsonPropertyName("id")]
  public Guid Id { get; }
}
