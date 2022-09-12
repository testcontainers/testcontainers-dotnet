namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Text.Json;
  using DotNet.Testcontainers.Images;

  internal abstract class CustomConfiguration
  {
    private readonly IReadOnlyDictionary<string, string> properties;

    protected CustomConfiguration(IReadOnlyDictionary<string, string> properties)
    {
      this.properties = properties;
    }

    protected string GetDockerConfig(string propertyName)
    {
      return this.GetPropertyValue(propertyName);
    }

    protected Uri GetDockerHost(string propertyName)
    {
      return this.properties.TryGetValue(propertyName, out var propertyValue) && Uri.TryCreate(propertyValue, UriKind.RelativeOrAbsolute, out var dockerHost) ? dockerHost : null;
    }

    protected JsonDocument GetDockerAuthConfig(string propertyName)
    {
      _ = this.properties.TryGetValue(propertyName, out var propertyValue);

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
      return this.GetPropertyValue(propertyName);
    }

    protected bool GetDockerTls(string propertyName)
    {
      _ = this.properties.TryGetValue(propertyName, out var propertyValue);
      return "1".Equals(propertyValue, StringComparison.Ordinal) || (bool.TryParse(propertyValue, out var tlsEnabled) && tlsEnabled);
    }

    protected bool GetDockerTlsVerify(string propertyName)
    {
      _ = this.properties.TryGetValue(propertyName, out var propertyValue);
      return "1".Equals(propertyValue, StringComparison.Ordinal) || (bool.TryParse(propertyValue, out var tlsVerifyEnabled) && tlsVerifyEnabled);
    }

    protected bool GetRyukDisabled(string propertyName)
    {
      return this.properties.TryGetValue(propertyName, out var propertyValue) && bool.TryParse(propertyValue, out var ryukDisabled) && ryukDisabled;
    }

    protected IDockerImage GetRyukContainerImage(string propertyName)
    {
      _ = this.properties.TryGetValue(propertyName, out var propertyValue);

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
      return this.GetPropertyValue(propertyName);
    }

    private string GetPropertyValue(string propertyName)
    {
      _ = this.properties.TryGetValue(propertyName, out var propertyValue);
      return propertyValue;
    }
  }
}
