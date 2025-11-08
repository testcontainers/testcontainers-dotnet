namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Text.Json;
  using DotNet.Testcontainers.Images;

  internal abstract class CustomConfiguration
  {
    private readonly IReadOnlyDictionary<string, string> _properties;

    protected CustomConfiguration(IReadOnlyDictionary<string, string> properties)
    {
      _properties = properties;
    }

    protected virtual string GetDockerConfig(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected virtual Uri GetDockerHost(string propertyName)
    {
      return _properties.TryGetValue(propertyName, out var propertyValue) && !string.IsNullOrEmpty(propertyValue) && Uri.TryCreate(propertyValue, UriKind.RelativeOrAbsolute, out var dockerHost) ? dockerHost : null;
    }

    protected virtual string GetDockerContext(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected virtual string GetDockerHostOverride(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected virtual string GetDockerSocketOverride(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected virtual JsonDocument GetDockerAuthConfig(string propertyName)
    {
      _ = _properties.TryGetValue(propertyName, out var propertyValue);

      if (string.IsNullOrEmpty(propertyValue))
      {
        return null;
      }

      try
      {
        return JsonDocument.Parse(propertyValue);
      }
      catch (Exception)
      {
        return null;
      }
    }

    protected virtual string GetDockerCertPath(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected virtual bool GetDockerTls(string propertyName)
    {
      return GetPropertyValue<bool>(propertyName);
    }

    protected virtual bool GetDockerTlsVerify(string propertyName)
    {
      return GetPropertyValue<bool>(propertyName);
    }

    protected virtual bool GetRyukDisabled(string propertyName)
    {
      return GetPropertyValue<bool>(propertyName);
    }

    protected virtual bool? GetRyukContainerPrivileged(string propertyName)
    {
      return GetPropertyValue<bool?>(propertyName);
    }

    protected virtual IImage GetRyukContainerImage(string propertyName)
    {
      _ = _properties.TryGetValue(propertyName, out var propertyValue);

      if (string.IsNullOrEmpty(propertyValue))
      {
        return null;
      }

      try
      {
        return new DockerImage(propertyValue);
      }
      catch (ArgumentException)
      {
        return null;
      }
    }

    protected virtual string GetHubImageNamePrefix(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected virtual ushort? GetWaitStrategyRetries(string propertyName)
    {
      return GetPropertyValue<ushort?>(propertyName);
    }

    protected virtual TimeSpan? GetWaitStrategyInterval(string propertyName)
    {
      return GetPropertyValue<TimeSpan?>(propertyName);
    }

    protected virtual TimeSpan? GetWaitStrategyTimeout(string propertyName)
    {
      return GetPropertyValue<TimeSpan?>(propertyName);
    }

    protected virtual TimeSpan? GetNamedPipeConnectionTimeout(string propertyName)
    {
      return GetPropertyValue<TimeSpan?>(propertyName);
    }

    private T GetPropertyValue<T>(string propertyName)
    {
      var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

      var isNullable = type != typeof(T);

      var hasValue = _properties.TryGetValue(propertyName, out var propertyValue);

      if (typeof(TimeSpan) == type)
      {
        return (T)(object)(hasValue && TimeSpan.TryParse(propertyValue, CultureInfo.InvariantCulture, out var result) && result > TimeSpan.Zero ? result : null);
      }

      switch (Type.GetTypeCode(type))
      {
        case TypeCode.Boolean:
        {
          return (T)(object)(hasValue && bool.TryParse(propertyValue, out var result) ? result : isNullable ? null : "1".Equals(propertyValue, StringComparison.Ordinal));
        }

        case TypeCode.UInt16:
        {
          return (T)(object)(hasValue && ushort.TryParse(propertyValue, out var result) ? result : isNullable ? null : 0);
        }

        case TypeCode.String:
        {
          return (T)(object)(string.IsNullOrEmpty(propertyValue) ? null : propertyValue);
        }

        default:
          throw new ArgumentOutOfRangeException(typeof(T).Name);
      }
    }
  }
}
