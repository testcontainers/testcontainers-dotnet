namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Runtime.InteropServices;
  using System.Security.Cryptography;
  using System.Text;
  using System.Text.Json;
  using System.Text.Json.Serialization;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a Docker config.
  /// </summary>
  internal sealed class DockerConfig
  {
    private static readonly string UserProfileDockerConfigDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker");

    private readonly ICustomConfiguration[] _customConfigurations;

    private readonly string _dockerConfigDirectoryPath;

    private readonly string _dockerConfigFilePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConfig" /> class.
    /// </summary>
    [PublicAPI]
    public DockerConfig()
      : this(EnvironmentConfiguration.Instance, PropertiesFileConfiguration.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConfig" /> class.
    /// </summary>
    /// <param name="customConfigurations">A list of custom configurations.</param>
    [PublicAPI]
    public DockerConfig(params ICustomConfiguration[] customConfigurations)
    {
      _customConfigurations = customConfigurations;
      _dockerConfigDirectoryPath = GetDockerConfig();
      _dockerConfigFilePath = Path.Combine(_dockerConfigDirectoryPath, "config.json");
    }

    /// <summary>
    /// Gets the <see cref="DockerConfig" /> instance.
    /// </summary>
    public static DockerConfig Instance { get; }
      = new DockerConfig();

    /// <inheritdoc cref="FileSystemInfo.Exists" />
    public bool Exists => File.Exists(_dockerConfigFilePath);

    /// <inheritdoc cref="FileSystemInfo.FullName" />
    public string FullName => _dockerConfigFilePath;

    /// <summary>
    /// Parses the Docker config file.
    /// </summary>
    /// <returns>A <see cref="JsonDocument" /> representing the Docker config.</returns>
    public JsonDocument Parse()
    {
      using (var dockerConfigFileStream = File.OpenRead(_dockerConfigFilePath))
      {
        return JsonDocument.Parse(dockerConfigFileStream);
      }
    }

    /// <summary>
    /// Gets the current Docker endpoint.
    /// </summary>
    /// <remarks>
    /// See the Docker CLI implementation <a href="https://github.com/docker/cli/blob/v25.0.0/cli/command/cli.go#L364-L390">comments</a>.
    /// Executes a command equivalent to <c>docker context inspect --format {{.Endpoints.docker.Host}}</c>.
    /// </remarks>
    /// A <see cref="Uri" /> representing the current Docker endpoint if available; otherwise, <c>null</c>.
    [CanBeNull]
    public Uri GetCurrentEndpoint()
    {
      const string defaultDockerContext = "default";

      var dockerHost = GetDockerHost();
      if (dockerHost != null)
      {
        return dockerHost;
      }

      var dockerContext = GetCurrentContext();
      if (string.IsNullOrEmpty(dockerContext) || defaultDockerContext.Equals(dockerContext))
      {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? NpipeEndpointAuthenticationProvider.DockerEngine : UnixEndpointAuthenticationProvider.DockerEngine;
      }

      using (var sha256 = SHA256.Create())
      {
        var dockerContextHash = BitConverter.ToString(sha256.ComputeHash(Encoding.Default.GetBytes(dockerContext))).Replace("-", string.Empty).ToLowerInvariant();
        var metaFilePath = Path.Combine(_dockerConfigDirectoryPath, "contexts", "meta", dockerContextHash, "meta.json");

        if (!File.Exists(metaFilePath))
        {
          return null;
        }

        using (var metaFileStream = File.OpenRead(metaFilePath))
        {
          var meta = JsonSerializer.Deserialize(metaFileStream, SourceGenerationContext.Default.DockerContextMeta);
          var host = meta?.Name == dockerContext ? meta.Endpoints?.Docker?.Host : null;
          return string.IsNullOrEmpty(host) ? null : new Uri(host.Replace("npipe:////./", "npipe://./"));
        }
      }
    }

    [CanBeNull]
    private string GetCurrentContext()
    {
      var dockerContext = GetDockerContext();
      if (!string.IsNullOrEmpty(dockerContext))
      {
        return dockerContext;
      }

      if (!Exists)
      {
        return null;
      }

      using (var dockerConfigJsonDocument = Parse())
      {
        if (dockerConfigJsonDocument.RootElement.TryGetProperty("currentContext", out var currentContext) && currentContext.ValueKind == JsonValueKind.String)
        {
          return currentContext.GetString();
        }
        else
        {
          return null;
        }
      }
    }

    [NotNull]
    private string GetDockerConfig()
    {
      var dockerConfigDirectoryPath = _customConfigurations.Select(customConfiguration => customConfiguration.GetDockerConfig()).FirstOrDefault(dockerConfig => !string.IsNullOrEmpty(dockerConfig));
      return dockerConfigDirectoryPath ?? UserProfileDockerConfigDirectoryPath;
    }

    [CanBeNull]
    private Uri GetDockerHost()
    {
      return _customConfigurations.Select(customConfiguration => customConfiguration.GetDockerHost()).FirstOrDefault(dockerHost => dockerHost != null);
    }

    [CanBeNull]
    private string GetDockerContext()
    {
      return _customConfigurations.Select(customConfiguration => customConfiguration.GetDockerContext()).FirstOrDefault(dockerContext => !string.IsNullOrEmpty(dockerContext));
    }

    internal sealed class DockerContextMeta
    {
      [JsonConstructor]
      public DockerContextMeta(string name, DockerContextMetaEndpoints endpoints)
      {
        Name = name;
        Endpoints = endpoints;
      }

      [JsonPropertyName("Name")]
      public string Name { get; }

      [JsonPropertyName("Endpoints")]
      public DockerContextMetaEndpoints Endpoints { get; }
    }

    internal sealed class DockerContextMetaEndpoints
    {
      [JsonConstructor]
      public DockerContextMetaEndpoints(DockerContextMetaEndpointsDocker docker)
      {
        Docker = docker;
      }

      [JsonPropertyName("docker")]
      public DockerContextMetaEndpointsDocker Docker { get; }
    }

    internal sealed class DockerContextMetaEndpointsDocker
    {
      [JsonConstructor]
      public DockerContextMetaEndpointsDocker(string host)
      {
        Host = host;
      }

      [JsonPropertyName("Host")]
      public string Host { get; }
    }
  }
}
