namespace DotNet.Testcontainers.Builders
{
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal sealed class CredsStoreProvider : IDockerRegistryAuthenticationProvider
  {
    private readonly JsonElement _rootElement;

    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CredsStoreProvider" /> class.
    /// </summary>
    /// <param name="jsonDocument">The JSON document that holds the Docker config credsStore node.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public CredsStoreProvider(JsonDocument jsonDocument, ILogger logger)
      : this(jsonDocument.RootElement, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CredsStoreProvider" /> class.
    /// </summary>
    /// <param name="jsonElement">The JSON element that holds the Docker config credsStore node.</param>
    /// <param name="logger">The logger.</param>
    [PublicAPI]
    public CredsStoreProvider(JsonElement jsonElement, ILogger logger)
    {
      _rootElement = jsonElement.TryGetProperty("credsStore", out var credsStore) ? credsStore : default;
      _logger = logger;
    }

    /// <inheritdoc />
    public bool IsApplicable(string hostname)
    {
      return !JsonValueKind.Undefined.Equals(_rootElement.ValueKind) && !string.IsNullOrEmpty(_rootElement.GetString());
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      _logger.SearchingDockerRegistryCredential("CredsStore");

      if (!IsApplicable(hostname))
      {
        return null;
      }

      var credentialProviderOutput = DockerCredentialProcess.Get(_rootElement.GetString(), hostname);
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
