namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.IO;
  using System.Text.Json;
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

    private readonly FileInfo _file;

    public DockerConfig() : this(GetFile())
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
              try
              {
                var endpoint = GetEndpoint(metaDirectory, currentContext);
                if (endpoint != null)
                {
                  return endpoint;
                }
              }
              catch
              {
                // try next meta.json file
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
      var metaFilePath = Path.Combine(metaDirectory, "meta.json");
      using (var metaFileStream = new FileStream(metaFilePath, FileMode.Open, FileAccess.Read))
      {
        using (var meta = JsonDocument.Parse(metaFileStream))
        {
          if (meta.RootElement.TryGetProperty("Name", out var nameNode) && nameNode.ValueKind == JsonValueKind.String && nameNode.GetString() == currentContext)
          {
            if (meta.RootElement.TryGetProperty("Endpoints", out var endpointsNode) && endpointsNode.ValueKind == JsonValueKind.Object)
            {
              if (endpointsNode.TryGetProperty("docker", out var dockerNode) && dockerNode.ValueKind == JsonValueKind.Object)
              {
                if (dockerNode.TryGetProperty("Host", out var hostNode) && hostNode.ValueKind == JsonValueKind.String)
                {
                  var host = hostNode.GetString();
                  if (!string.IsNullOrEmpty(host))
                  {
                    return new Uri(host);
                  }
                }
              }
            }
          }
        }
      }

      return null;
    }
  }
}
