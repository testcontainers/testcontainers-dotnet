namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  public interface IWaitWhile
  {
    Task<bool> While(ITestcontainersContainer testcontainers, ILogger logger);
  }
}
