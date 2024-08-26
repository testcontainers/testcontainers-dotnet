namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Linq;
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

    private static readonly string UserProfileDockerContextMetaDirectoryPath = Path.Combine(UserProfileDockerConfigDirectoryPath, "contexts", "meta");

    private readonly FileInfo _dockerConfigFile;

    private readonly ICustomConfiguration[] _customConfigurations;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConfig" /> class.
    /// </summary>
    [PublicAPI]
    public DockerConfig()
      : this(GetDockerConfigFile(), EnvironmentConfiguration.Instance, PropertiesFileConfiguration.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConfig" /> class.
    /// </summary>
    /// <param name="customConfigurations">A list of custom configurations.</param>
    [PublicAPI]
    public DockerConfig(params ICustomConfiguration[] customConfigurations)
      : this(GetDockerConfigFile(), customConfigurations)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConfig" /> class.
    /// </summary>
    /// <param name="dockerConfigFile">The Docker config file.</param>
    /// <param name="customConfigurations">A list of custom configurations.</param>
    [PublicAPI]
    public DockerConfig(FileInfo dockerConfigFile, params ICustomConfiguration[] customConfigurations)
    {
      _dockerConfigFile = dockerConfigFile;
      _customConfigurations = customConfigurations;
    }

    /// <summary>
    /// Gets the <see cref="DockerConfig" /> instance.
    /// </summary>
    public static DockerConfig Instance { get; }
      = new DockerConfig();

    /// <inheritdoc cref="FileInfo.Exists" />
    public bool Exists => _dockerConfigFile.Exists;

    /// <inheritdoc cref="FileInfo.Exists" />
    public string FullName => _dockerConfigFile.FullName;

    /// <summary>
    /// Parses the Docker config file.
    /// </summary>
    /// <returns>A <see cref="JsonDocument" /> representing the Docker config.</returns>
    public JsonDocument Parse()
    {
      using (var dockerConfigFileStream = _dockerConfigFile.OpenRead())
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

      var dockerContext = GetCurrentDockerContext();
      if (string.IsNullOrEmpty(dockerContext) || defaultDockerContext.Equals(dockerContext))
      {
        return TestcontainersSettings.OS.DockerEndpointAuthConfig.Endpoint;
      }

      using (var sha256 = SHA256.Create())
      {
        var dockerContextHash = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(dockerContext))).Replace("-", string.Empty).ToLowerInvariant();
        var metaFilePath = Path.Combine(UserProfileDockerContextMetaDirectoryPath, dockerContextHash, "meta.json");

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
    private string GetCurrentDockerContext()
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

      using (var config = Parse())
      {
        if (config.RootElement.TryGetProperty("currentContext", out var currentContextNode) && currentContextNode.ValueKind == JsonValueKind.String)
        {
          return currentContextNode.GetString();
        }
        else
        {
          return null;
        }
      }
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

    private static FileInfo GetDockerConfigFile()
    {
      var dockerConfigDirectoryPath = EnvironmentConfiguration.Instance.GetDockerConfig() ?? PropertiesFileConfiguration.Instance.GetDockerConfig() ?? UserProfileDockerConfigDirectoryPath;
      return new FileInfo(Path.Combine(dockerConfigDirectoryPath, "config.json"));
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
