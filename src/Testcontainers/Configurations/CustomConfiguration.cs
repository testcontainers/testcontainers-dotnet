namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Text.Json;
  using DotNet.Testcontainers.Images;

  internal abstract class CustomConfiguration
  {
    private readonly IReadOnlyDictionary<string, string> _properties;

    protected CustomConfiguration(IReadOnlyDictionary<string, string> properties)
    {
      _properties = properties;
    }

    protected string GetDockerConfig(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected Uri GetDockerHost(string propertyName)
    {
      return _properties.TryGetValue(propertyName, out var propertyValue) && Uri.TryCreate(propertyValue, UriKind.RelativeOrAbsolute, out var dockerHost) ? dockerHost : null;
    }

    protected string GetDockerHostOverride(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected string GetDockerSocketOverride(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected JsonDocument GetDockerAuthConfig(string propertyName)
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

    protected string GetDockerCertPath(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    protected bool GetDockerTls(string propertyName)
    {
      return GetPropertyValue<bool>(propertyName);
    }

    protected bool GetDockerTlsVerify(string propertyName)
    {
      return GetPropertyValue<bool>(propertyName);
    }

    protected bool GetRyukDisabled(string propertyName)
    {
      return GetPropertyValue<bool>(propertyName);
    }

    protected bool GetRyukContainerPrivileged(string propertyName)
    {
      return GetPropertyValue<bool>(propertyName);
    }

    protected IImage GetRyukContainerImage(string propertyName)
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

    protected string GetHubImageNamePrefix(string propertyName)
    {
      return GetPropertyValue<string>(propertyName);
    }

    private T GetPropertyValue<T>(string propertyName)
    {
      switch (Type.GetTypeCode(typeof(T)))
      {
        case TypeCode.Boolean:
        {
          return (T)(object)(_properties.TryGetValue(propertyName, out var propertyValue) && ("1".Equals(propertyValue, StringComparison.Ordinal) || (bool.TryParse(propertyValue, out var result) && result)));
        }

        case TypeCode.String:
        {
          _ = _properties.TryGetValue(propertyName, out var propertyValue);
          return (T)(object)propertyValue;
        }

        default:
          throw new ArgumentOutOfRangeException(typeof(T).Name);
      }
    }
  }
}
