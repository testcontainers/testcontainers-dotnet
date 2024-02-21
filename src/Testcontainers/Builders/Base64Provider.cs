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
      : this(jsonDocument.RootElement, logger)
    {
    }

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
    /// Gets a predicate that determines whether or not a <see cref="JsonProperty" /> contains a Docker registry key.
    /// </summary>
    public static Func<JsonProperty, string, bool> HasDockerRegistryKey { get; }
      = (property, hostname) => property.Name.Equals(hostname, StringComparison.OrdinalIgnoreCase) || property.Name.EndsWith("://" + hostname, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public bool IsApplicable(string hostname)
    {
      return !default(JsonElement).Equals(_rootElement) && !JsonValueKind.Null.Equals(_rootElement.ValueKind) && _rootElement.EnumerateObject().Any(property => HasDockerRegistryKey(property, hostname));
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      _logger.SearchingDockerRegistryCredential("Auths");

      if (!IsApplicable(hostname))
      {
        return null;
      }

      var authProperty = _rootElement.EnumerateObject().LastOrDefault(property => HasDockerRegistryKey(property, hostname));

      if (JsonValueKind.Undefined.Equals(authProperty.Value.ValueKind))
      {
        return null;
      }

      if (authProperty.Value.TryGetProperty("identitytoken", out var identityToken) && JsonValueKind.String.Equals(identityToken.ValueKind))
      {
        var identityTokenValue = identityToken.GetString();

        if (!string.IsNullOrEmpty(identityTokenValue))
        {
          _logger.DockerRegistryCredentialFound(hostname);
          return new DockerRegistryAuthenticationConfiguration(authProperty.Name, null, null, identityTokenValue);
        }
      }

      if (!authProperty.Value.TryGetProperty("auth", out var auth))
      {
        return null;
      }

      if (!JsonValueKind.String.Equals(auth.ValueKind) && !JsonValueKind.Null.Equals(auth.ValueKind))
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
      return new DockerRegistryAuthenticationConfiguration(authProperty.Name, credential[0], credential[1]);
    }
  }
}
