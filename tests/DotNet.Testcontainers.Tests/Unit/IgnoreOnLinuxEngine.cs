namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Clients;
  using Xunit;

  public sealed class IgnoreOnLinuxEngine : FactAttribute
  {
    private static readonly bool IsLinuxEngineEnabled;

    static IgnoreOnLinuxEngine()
    {
      var testcontainersClient = new TestcontainersClient();
      IsLinuxEngineEnabled = !testcontainersClient.GetIsWindowsEngineEnabled().GetAwaiter().GetResult();
    }

    public IgnoreOnLinuxEngine()
    {
      if (IsLinuxEngineEnabled)
      {
        this.Skip = "Windows Docker engine is not available.";
      }
    }
  }
}
