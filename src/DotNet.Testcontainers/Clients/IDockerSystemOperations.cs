namespace DotNet.Testcontainers.Clients
{
  using System.Threading.Tasks;

  internal interface IDockerSystemOperations
  {
    Task<bool> GetIsWindowsEngineEnabled();
  }
}
