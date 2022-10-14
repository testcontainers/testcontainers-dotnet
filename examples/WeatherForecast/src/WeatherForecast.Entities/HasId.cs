using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

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
