namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Configurations;

  public interface IWaitWhile
  {
    Task<bool> While(IDockerClientConfiguration clientAuthConfig, string id);
  }
}
