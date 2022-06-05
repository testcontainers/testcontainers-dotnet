namespace DotNet.Testcontainers.Builders
{
  using System.Diagnostics;
  using System.Text.Json;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IAuthenticationProvider" />
  internal sealed class CredsStoreProvider : IAuthenticationProvider
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
    public bool IsApplicable()
    {
      return !default(JsonElement).Equals(this.rootElement) && !string.IsNullOrEmpty(this.rootElement.GetString());
    }

    /// <inheritdoc />
    public IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname)
    {
      this.logger.SearchingDockerRegistryCredential("CredsStore");

      if (!this.IsApplicable())
      {
        return null;
      }

      var dockerCredentialStartInfo = new ProcessStartInfo();
      dockerCredentialStartInfo.FileName = "docker-credential-" + this.rootElement.GetString();
      dockerCredentialStartInfo.Arguments = "get";
      dockerCredentialStartInfo.RedirectStandardInput = true;
      dockerCredentialStartInfo.RedirectStandardOutput = true;
      dockerCredentialStartInfo.UseShellExecute = false;

      using (var dockerCredentialProcess = Process.Start(dockerCredentialStartInfo))
      {
        if (dockerCredentialProcess == null)
        {
          return null;
        }

        dockerCredentialProcess.StandardInput.WriteLine(hostname);
        dockerCredentialProcess.StandardInput.Close();

        var stdOut = dockerCredentialProcess.StandardOutput.ReadToEnd();
        AuthConfig authConfig;

        try
        {
          authConfig = JsonSerializer.Deserialize<AuthConfig>(JsonDocument.Parse(stdOut).RootElement.GetRawText(), JsonSerializerOptions);
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
}
