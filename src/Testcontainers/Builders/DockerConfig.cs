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

  internal class DockerConfig
  {
    private static readonly string UserProfileDockerConfigDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docker");

    private static readonly string UserProfileDockerContextMetaPath = Path.Combine(UserProfileDockerConfigDirectoryPath, "contexts", "meta");

    private static FileInfo GetFile()
    {
      var dockerConfigDirectoryPath = EnvironmentConfiguration.Instance.GetDockerConfig() ?? PropertiesFileConfiguration.Instance.GetDockerConfig() ?? UserProfileDockerConfigDirectoryPath;
      return new FileInfo(Path.Combine(dockerConfigDirectoryPath, "config.json"));
    }

    public static DockerConfig Default { get; } = new DockerConfig();

    private readonly FileInfo _file;
    private readonly ICustomConfiguration _environment;

    private DockerConfig() : this(GetFile(), EnvironmentConfiguration.Instance)
    {
    }

    public DockerConfig(ICustomConfiguration environment) : this(GetFile(), environment)
    {
    }

    public DockerConfig(FileInfo file, ICustomConfiguration environment = null)
    {
      _file = file;
      _environment = environment ?? EnvironmentConfiguration.Instance;
    }

    public bool Exists => _file.Exists;

    public string FullName => _file.FullName;

    public JsonDocument Parse()
    {
      using (var dockerConfigFileStream = _file.OpenRead())
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
        var envDockerHost = _environment.GetDockerHost();
        if (envDockerHost != null)
        {
          return envDockerHost;
        }

        var currentContext = GetCurrentContext();
        if (currentContext is null or "default")
        {
          return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? NpipeEndpointAuthenticationProvider.DockerEngine : UnixEndpointAuthenticationProvider.DockerEngine;
        }

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(currentContext));
        var digest = hash.Aggregate(new StringBuilder(hash.Length * 2), (sb, b) => sb.Append(b.ToString("x2"))).ToString();
        var contextDirectory = Path.Combine(UserProfileDockerContextMetaPath, digest);
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
      var dockerContext = Environment.GetEnvironmentVariable("DOCKER_CONTEXT");
      if (!string.IsNullOrEmpty(dockerContext))
      {
        return dockerContext;
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
