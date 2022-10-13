using System;
using JetBrains.Annotations;

namespace WeatherForecast.Entities;

[PublicAPI]
public readonly struct Temperature
{
  private Temperature(string unitName, string unitSymbol, double value, DateTime measured)
  {
    UnitName = unitName;
    UnitSymbol = unitSymbol;
    Value = value;
    Measured = measured;
  }

  public static Temperature Kelvin(double value, DateTime measured)
  {
    return new Temperature("Kelvin", "K", value, measured);
  }

  public static Temperature Celsius(double value, DateTime measured)
  {
    return new Temperature("degree Celsius", "°C", value, measured);
  }

  public static Temperature Fahrenheit(double value, DateTime measured)
  {
    return new Temperature("degree Fahrenheit", "°F", value, measured);
  }

  public string UnitName { get; }

  public string UnitSymbol { get; }

  public double Value { get; }

  public DateTime Measured { get; }
}
