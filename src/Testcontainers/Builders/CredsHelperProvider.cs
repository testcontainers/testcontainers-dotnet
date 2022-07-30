namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Linq;
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal sealed class CredsHelperProvider : IDockerRegistryAuthenticationProvider
  {
    private readonly JsonElement rootElement;

    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CredsHelperProvider" /> class.
    /// </summary>
    /// <param name="jsonDocument">The JSON document that holds the Docker config credsHelper node.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public CredsHelperProvider(JsonDocument jsonDocument, ILogger logger)
      : this(jsonDocument.RootElement, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CredsHelperProvider" /> class.
    /// </summary>
    /// <param name="jsonElement">The JSON element that holds the Docker config credsHelper node.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public CredsHelperProvider(JsonElement jsonElement, ILogger logger)
    {
      this.rootElement = jsonElement.TryGetProperty("credHelpers", out var credHelpers) ? credHelpers : default;
      this.logger = logger;
    }

    /// <inheritdoc />
    public bool IsApplicable(string hostname)
    {
#if NETSTANDARD2_1_OR_GREATER
      return !default(JsonElement).Equals(this.rootElement) && !JsonValueKind.Null.Equals(this.rootElement.ValueKind) && this.rootElement.EnumerateObject().Any(property => property.Name.Contains(hostname, StringComparison.OrdinalIgnoreCase));
#else
      return !default(JsonElement).Equals(this.rootElement) && !JsonValueKind.Null.Equals(this.rootElement.ValueKind) && this.rootElement.EnumerateObject().Any(property => property.Name.IndexOf(hostname, StringComparison.OrdinalIgnoreCase) >= 0);
#endif
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      this.logger.SearchingDockerRegistryCredential("CredHelpers");

      if (!this.IsApplicable(hostname))
      {
        return null;
      }

#if NETSTANDARD2_1_OR_GREATER
      var registryEndpointProperty = this.rootElement.EnumerateObject().LastOrDefault(property => property.Name.Contains(hostname, StringComparison.OrdinalIgnoreCase));
#else
      var registryEndpointProperty = this.rootElement.EnumerateObject().LastOrDefault(property => property.Name.IndexOf(hostname, StringComparison.OrdinalIgnoreCase) >= 0);
#endif

      if (!JsonValueKind.String.Equals(registryEndpointProperty.Value.ValueKind))
      {
        return null;
      }

      if (string.IsNullOrEmpty(registryEndpointProperty.Value.GetString()))
      {
        return null;
      }

      var credentialProviderOutput = DockerCredentialProcess.Get(registryEndpointProperty.Value.GetString(), hostname);
      if (string.IsNullOrEmpty(credentialProviderOutput))
      {
        return null;
      }

      JsonElement credential;

      try
      {
        credential = JsonDocument.Parse(credentialProviderOutput).RootElement;
      }
      catch (JsonException)
      {
        return null;
      }

      this.logger.DockerRegistryCredentialFound(hostname);
      return new DockerRegistryAuthenticationConfiguration(hostname, credential);
    }
  }
}
