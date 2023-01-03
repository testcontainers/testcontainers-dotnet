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
    private readonly JsonElement rootElement;

    private readonly ILogger logger;

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
      this.rootElement = jsonElement.TryGetProperty("auths", out var auths) ? auths : default;
      this.logger = logger;
    }

    /// <summary>
    /// Gets a predicate that determines whether or not a <see cref="JsonProperty" /> contains a Docker registry key.
    /// </summary>
    public static Func<JsonProperty, string, bool> HasDockerRegistryKey { get; }
      = (property, hostname) => property.Name.Equals(hostname, StringComparison.OrdinalIgnoreCase) || property.Name.EndsWith("://" + hostname, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public bool IsApplicable(string hostname)
    {
      return !default(JsonElement).Equals(this.rootElement) && !JsonValueKind.Null.Equals(this.rootElement.ValueKind) && this.rootElement.EnumerateObject().Any(property => HasDockerRegistryKey(property, hostname));
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      this.logger.SearchingDockerRegistryCredential("Auths");

      if (!this.IsApplicable(hostname))
      {
        return null;
      }

      var authProperty = this.rootElement.EnumerateObject().LastOrDefault(property => HasDockerRegistryKey(property, hostname));

      if (JsonValueKind.Undefined.Equals(authProperty.Value.ValueKind))
      {
        return null;
      }

      if (!authProperty.Value.TryGetProperty("auth", out var auth))
      {
        return null;
      }

      if (string.IsNullOrEmpty(auth.GetString()))
      {
        return null;
      }

      var credentialInBytes = Convert.FromBase64String(auth.GetString());
      var credential = Encoding.UTF8.GetString(credentialInBytes).Split(new[] { ':' }, 2);

      if (credential.Length != 2)
      {
        return null;
      }

      this.logger.DockerRegistryCredentialFound(hostname);
      return new DockerRegistryAuthenticationConfiguration(authProperty.Name, credential[0], credential[1]);
    }
  }
}
