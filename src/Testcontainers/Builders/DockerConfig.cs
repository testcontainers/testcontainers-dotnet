namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
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

    private DockerConfig() : this(GetFile())
    {
    }

    public DockerConfig(FileInfo file)
    {
      _file = file;
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

    [CanBeNull]
    public Uri GetCurrentEndpoint()
    {
      try
      {
        using (var config = Parse())
        {
          if (config.RootElement.TryGetProperty("currentContext", out var currentContextNode) && currentContextNode.ValueKind == JsonValueKind.String)
          {
            var currentContext = currentContextNode.GetString();
            foreach (var metaDirectory in Directory.EnumerateDirectories(UserProfileDockerContextMetaPath, "*", SearchOption.TopDirectoryOnly))
            {
              var endpoint = GetEndpoint(metaDirectory, currentContext);
              if (endpoint != null)
              {
                return endpoint;
              }
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
              return new Uri(host);
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
