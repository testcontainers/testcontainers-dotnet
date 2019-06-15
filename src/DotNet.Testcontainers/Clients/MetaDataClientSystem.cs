namespace DotNet.Testcontainers.Clients
{
  using System;

  internal sealed class MetaDataClientSystem : DockerApiClient
  {
    private static readonly Lazy<MetaDataClientSystem> MetaDataClient = new Lazy<MetaDataClientSystem>(() => new MetaDataClientSystem());

    private MetaDataClientSystem()
    {
    }

    internal static MetaDataClientSystem Instance
    {
      get
      {
        return MetaDataClient.Value;
      }
    }

    internal bool IsWindowsEngineEnabled { get; } = "Windows_NT".Equals(Docker.System.GetSystemInfoAsync().Result.OperatingSystem);
  }
}
