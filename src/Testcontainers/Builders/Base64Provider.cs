namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Linq;
  using System.Text;
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal sealed class Base64Provider : IDockerRegistryAuthenticationProvider
  {
    private readonly JsonElement _rootElement;

    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Base64Provider" /> class.
    /// </summary>
    /// <param name="jsonDocument">The JSON document that holds the Docker config auths node.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public Base64Provider(JsonDocument jsonDocument, ILogger logger)
      : this(jsonDocument.RootElement, logger) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Base64Provider" /> class.
    /// </summary>
    /// <param name="jsonElement">The JSON element that holds the Docker config auths node.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public Base64Provider(JsonElement jsonElement, ILogger logger)
    {
      _rootElement = jsonElement.TryGetProperty("auths", out var auths) ? auths : default;
      _logger = logger;
    }

    /// <summary>
    /// Determines whether the specified JSON property contains a Docker registry
    /// that matches the given registry host.
    /// </summary>
    /// <param name="property">The JSON property to check.</param>
    /// <param name="registryHost">The registry host to match against.</param>
    /// <returns><c>true</c> if the property contains a matching Docker registry; otherwise, <c>false</c>.</returns>
    public static bool HasDockerRegistryName(JsonProperty property, string registryHost)
    {
      var propertyName = property.Name;

      if (propertyName.Equals(registryHost, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      if (propertyName.EndsWith("://" + registryHost, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      if (
        TryGetHost(propertyName, out var propertyNameNormalized)
        && TryGetHost(registryHost, out var registryHostNormalized)
      )
      {
        return string.Equals(
          propertyNameNormalized,
          registryHostNormalized,
          StringComparison.OrdinalIgnoreCase
        );
      }
      else
      {
        return false;
      }
    }

    /// <inheritdoc />
    public bool IsApplicable(string hostname)
    {
      return !JsonValueKind.Undefined.Equals(_rootElement.ValueKind)
        && !JsonValueKind.Null.Equals(_rootElement.ValueKind)
        && _rootElement
          .EnumerateObject()
          .Any(property => HasDockerRegistryName(property, hostname));
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      _logger.SearchingDockerRegistryCredential("Auths");

      if (!IsApplicable(hostname))
      {
        return null;
      }

      var authProperty = _rootElement
        .EnumerateObject()
        .LastOrDefault(property => HasDockerRegistryName(property, hostname));

      if (JsonValueKind.Undefined.Equals(authProperty.Value.ValueKind))
      {
        return null;
      }

      if (
        authProperty.Value.TryGetProperty("identitytoken", out var identityToken)
        && JsonValueKind.String.Equals(identityToken.ValueKind)
      )
      {
        var identityTokenValue = identityToken.GetString();

        if (!string.IsNullOrEmpty(identityTokenValue))
        {
          _logger.DockerRegistryCredentialFound(hostname);
          return new DockerRegistryAuthenticationConfiguration(
            authProperty.Name,
            null,
            null,
            identityTokenValue
          );
        }
      }

      if (!authProperty.Value.TryGetProperty("auth", out var auth))
      {
        return null;
      }

      if (
        !JsonValueKind.String.Equals(auth.ValueKind) && !JsonValueKind.Null.Equals(auth.ValueKind)
      )
      {
        _logger.DockerRegistryAuthPropertyValueKindInvalid(hostname, auth.ValueKind);
        return null;
      }

      var authValue = auth.GetString();

      if (string.IsNullOrEmpty(authValue))
      {
        _logger.DockerRegistryAuthPropertyValueNotFound(hostname);
        return null;
      }

      byte[] credentialInBytes;

      try
      {
        credentialInBytes = Convert.FromBase64String(authValue);
      }
      catch (FormatException e)
      {
        _logger.DockerRegistryAuthPropertyValueInvalidBase64(hostname, e);
        return null;
      }

      var credential = Encoding.Default.GetString(credentialInBytes).Split(new[] { ':' }, 2);

      if (credential.Length != 2)
      {
        _logger.DockerRegistryAuthPropertyValueInvalidBasicAuthenticationFormat(hostname);
        return null;
      }

      _logger.DockerRegistryCredentialFound(hostname);
      return new DockerRegistryAuthenticationConfiguration(
        authProperty.Name,
        credential[0],
        credential[1]
      );
    }

    /// <summary>
    /// Tries to extract the host from the specified value.
    /// </summary>
    /// <param name="value">The string to extract the host from.</param>
    /// <param name="host">The extracted host if successful; otherwise, the original string.</param>
    /// <returns><c>true</c> if the host was successfully extracted; otherwise, <c>false</c>.</returns>
    private static bool TryGetHost(string value, out string host)
    {
      var uriToParse = value.Contains("://") ? value : "dummy://" + value;

      if (Uri.TryCreate(uriToParse, UriKind.Absolute, out var uri))
      {
        host = uri.Port == -1 || uri.IsDefaultPort ? uri.Host : uri.Host + ":" + uri.Port;
        return true;
      }
      else
      {
        host = value;
        return false;
      }
    }
  }
}
