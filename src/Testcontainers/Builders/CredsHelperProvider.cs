namespace DotNet.Testcontainers.Builders
{
  using System.Linq;
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal sealed class CredsHelperProvider : IDockerRegistryAuthenticationProvider
  {
    private readonly JsonElement _rootElement;

    private readonly ILogger _logger;

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
      _rootElement = jsonElement.TryGetProperty("credHelpers", out var credHelpers) ? credHelpers : default;
      _logger = logger;
    }

    /// <inheritdoc />
    public bool IsApplicable(string hostname)
    {
      return !JsonValueKind.Undefined.Equals(_rootElement.ValueKind) && !JsonValueKind.Null.Equals(_rootElement.ValueKind) && _rootElement.EnumerateObject().Any(property => Base64Provider.HasDockerRegistryKey(property, hostname));
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      _logger.SearchingDockerRegistryCredential("CredHelpers");

      if (!IsApplicable(hostname))
      {
        return null;
      }

      var registryEndpointProperty = _rootElement.EnumerateObject().LastOrDefault(property => Base64Provider.HasDockerRegistryKey(property, hostname));

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

      _logger.DockerRegistryCredentialFound(hostname);
      return new DockerRegistryAuthenticationConfiguration(hostname, credential);
    }
  }
}
