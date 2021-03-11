namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Configurations;

  public interface IWaitUntil
  {
    Task<bool> Until(IDockerClientAuthenticationConfiguration clientAuthConfig, string id);
  }
}
