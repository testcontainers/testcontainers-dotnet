namespace DotNet.Testcontainers.Builders
{
  using System.Text.Json;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IDockerEndpointAuthenticationProvider" />
  internal sealed class CredsStoreProvider : IDockerRegistryAuthenticationProvider
  {
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions();

    private readonly JsonElement rootElement;

    private readonly ILogger logger;

    static CredsStoreProvider()
    {
      JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    }

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
      this.rootElement = jsonElement.TryGetProperty("credsStore", out var credsStore) ? credsStore : default;
      this.logger = logger;
    }

    /// <inheritdoc />
    public bool IsApplicable(string hostname)
    {
      return !default(JsonElement).Equals(this.rootElement) && !string.IsNullOrEmpty(this.rootElement.GetString());
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      this.logger.SearchingDockerRegistryCredential("CredsStore");

      if (!this.IsApplicable(hostname))
      {
        return null;
      }

      var credentialProviderOutput = DockerCredentialProcess.Get(this.rootElement.GetString(), hostname);
      if (string.IsNullOrEmpty(credentialProviderOutput))
      {
        return null;
      }

      AuthConfig authConfig;

      try
      {
        authConfig = JsonSerializer.Deserialize<AuthConfig>(JsonDocument.Parse(credentialProviderOutput).RootElement.GetRawText(), JsonSerializerOptions);
      }
      catch (JsonException)
      {
        return null;
      }

      this.logger.DockerRegistryCredentialFound(hostname);
      return new DockerRegistryAuthenticationConfiguration(authConfig.ServerAddress, authConfig.Username, authConfig.Password);
    }
  }
}
