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
  ///
  /// </summary>
  internal sealed class DockerConfig
  {
    private static readonly string UserProfileDockerConfigDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker");

    private static readonly string UserProfileDockerContextMetaDirectoryPath = Path.Combine(UserProfileDockerConfigDirectoryPath, "contexts", "meta");

    private readonly FileInfo _dockerConfigFile;

    [CanBeNull]
    private readonly Uri _dockerHost;

    [CanBeNull]
    private readonly string _dockerContext;

    /// <summary>
    ///
    /// </summary>
    public DockerConfig()
      : this(GetDockerConfigFile())
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dockerConfigFile"></param>
    public DockerConfig(FileInfo dockerConfigFile)
      : this(dockerConfigFile, EnvironmentConfiguration.Instance, PropertiesFileConfiguration.Instance)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dockerConfigFile"></param>
    public DockerConfig(params ICustomConfiguration[] customConfigurations)
      : this(GetDockerConfigFile(), customConfigurations)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dockerConfigFile"></param>
    /// <param name="customConfigurations"></param>
    public DockerConfig(FileInfo dockerConfigFile, params ICustomConfiguration[] customConfigurations)
    {
      _dockerConfigFile = dockerConfigFile;
      _dockerHost = customConfigurations.Select(customConfiguration => customConfiguration.GetDockerHost()).FirstOrDefault(dockerHost => dockerHost != null);
      _dockerContext = customConfigurations.Select(customConfiguration => customConfiguration.GetDockerContext()).FirstOrDefault(dockerContext => !string.IsNullOrEmpty(dockerContext));
    }

    /// <summary>
    ///
    /// </summary>
    public static DockerConfig Instance { get; }
      = new DockerConfig(GetDockerConfigFile());

    /// <inheritdoc cref="FileInfo.Exists" />
    public bool Exists => _dockerConfigFile.Exists;

    /// <inheritdoc cref="FileInfo.Exists" />
    public string FullName => _dockerConfigFile.FullName;

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public JsonDocument Parse()
    {
      using (var dockerConfigFileStream = _dockerConfigFile.OpenRead())
      {
        return JsonDocument.Parse(dockerConfigFileStream);
      }
    }

    /// <summary>
    /// Performs the equivalent of running <c>docker context inspect --format {{.Endpoints.docker.Host}}</c>
    /// </summary>
    [CanBeNull]
    public Uri GetCurrentEndpoint()
    {
      try
      {
        if (_dockerHost != null)
        {
          return _dockerHost;
        }

        var currentContext = GetCurrentContext();
        if (currentContext is null or "default")
        {
          return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? NpipeEndpointAuthenticationProvider.DockerEngine : UnixEndpointAuthenticationProvider.DockerEngine;
        }

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(currentContext));
        var digest = hash.Aggregate(new StringBuilder(hash.Length * 2), (sb, b) => sb.Append(b.ToString("x2"))).ToString();
        var contextDirectory = Path.Combine(UserProfileDockerContextMetaDirectoryPath, digest);
        var endpoint = GetEndpoint(contextDirectory, currentContext);
        return endpoint;
      }
      catch
      {
        return null;
      }
    }

    [CanBeNull]
    private string GetCurrentContext()
    {
      if (!string.IsNullOrEmpty(_dockerContext))
      {
        return _dockerContext;
      }

      using (var config = Parse())
      {
        if (config.RootElement.TryGetProperty("currentContext", out var currentContextNode) && currentContextNode.ValueKind == JsonValueKind.String)
        {
          return currentContextNode.GetString();
        }

        return null;
      }
    }

    [CanBeNull]
    private static Uri GetEndpoint(string metaDirectory, string currentContext)
    {
      try
      {
        var metaFilePath = Path.Combine(metaDirectory, "meta.json");
        using (var metaFileStream = new FileStream(metaFilePath, FileMode.Open, FileAccess.Read))
        {
          var meta = JsonSerializer.Deserialize(metaFileStream, SourceGenerationContext.Default.DockerContextMeta);
          if (meta?.Name == currentContext)
          {
            var host = meta?.Endpoints?.Docker?.Host;
            if (!string.IsNullOrEmpty(host))
            {
              const string npipePrefix = "npipe:////./";
              return host.StartsWith(npipePrefix, StringComparison.Ordinal) ? new Uri($"npipe://./{host.Substring(npipePrefix.Length)}") : new Uri(host);
            }
          }
        }
      }
      catch
      {
        return null;
      }

      return null;
    }

    private static FileInfo GetDockerConfigFile()
    {
      var dockerConfigDirectoryPath = EnvironmentConfiguration.Instance.GetDockerConfig() ?? PropertiesFileConfiguration.Instance.GetDockerConfig() ?? UserProfileDockerConfigDirectoryPath;
      return new FileInfo(Path.Combine(dockerConfigDirectoryPath, "config.json"));
    }

    internal class DockerContextMeta
    {
      [JsonPropertyName("Name"), CanBeNull]
      public string Name { get; set; }

      [JsonPropertyName("Endpoints"), CanBeNull]
      public DockerContextMetaEndpoints Endpoints { get; set; }
    }

    internal class DockerContextMetaEndpoints
    {
      [JsonPropertyName("docker"), CanBeNull]
      public DockerContextMetaEndpointsDocker Docker { get; set; }
    }

    internal class DockerContextMetaEndpointsDocker
    {
      [JsonPropertyName("Host"), CanBeNull]
      public string Host { get; set; }
    }
  }
}
