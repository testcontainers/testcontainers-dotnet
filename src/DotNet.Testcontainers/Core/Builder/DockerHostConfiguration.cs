namespace DotNet.Testcontainers.Core.Builder
{
  using DotNet.Testcontainers.Clients;

  public static class DockerHostConfiguration
  {
    public static bool IsWindowsEngineEnabled => MetaDataClientSystem.Instance.IsWindowsEngineEnabled;
  }
}
