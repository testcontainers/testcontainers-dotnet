namespace DotNet.Testcontainers.Builders
{
  using System.Text.Json;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc />
  internal sealed class CredsHelperProvider : IDockerRegistryAuthenticationProvider
  {
    private const string TokenUsername = "<token>";
    private readonly JsonElement dockerConfig;
    private readonly ILogger logger;

    public CredsHelperProvider(JsonElement dockerConfig, ILogger logger)
    {
      this.dockerConfig = dockerConfig;
      this.logger = logger;
    }

    /// <inheritdoc />
    public bool IsApplicable(string hostname)
    {
      return this.FindCredHelperName(hostname) != null;
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      this.logger.SearchingDockerRegistryCredential("CredsHelper");
      var credHelperName = this.FindCredHelperName(hostname);
      if (credHelperName == null)
      {
        return null;
      }

      var credentialProviderOutput = ExternalProcessCredentialProvider.GetCredentialProviderOutput(credHelperName, hostname);
      if (credentialProviderOutput == null)
      {
        return null;
      }

      DockerRegistryAuthenticationConfiguration dockerRegistryAuthenticationConfiguration;
      try
      {
        var credentialProviderOutputJson = JsonSerializer.Deserialize<JsonElement>(credentialProviderOutput);
        var username = credentialProviderOutputJson.GetProperty("Username").GetString();
        var secret = credentialProviderOutputJson.GetProperty("Secret").GetString();
        dockerRegistryAuthenticationConfiguration = username == TokenUsername ? new DockerRegistryAuthenticationConfiguration(hostname, identityToken: secret) : new DockerRegistryAuthenticationConfiguration(hostname, username, secret);
      }
      catch (JsonException)
      {
        return null;
      }

      this.logger.DockerRegistryCredentialFound(hostname);
      return dockerRegistryAuthenticationConfiguration;
    }

    private string FindCredHelperName(string hostname)
    {
      return this.dockerConfig.TryGetProperty("credHelpers", out var credHelpersProperty) && credHelpersProperty.ValueKind != JsonValueKind.Null && credHelpersProperty.TryGetProperty(hostname, out var credHelperProperty)
        ? credHelperProperty.GetString()
        : null;
    }
  }
}
