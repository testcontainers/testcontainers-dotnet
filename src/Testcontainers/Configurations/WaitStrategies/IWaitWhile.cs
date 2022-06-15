namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  [PublicAPI]
  public interface IWaitWhile
  {
    [PublicAPI]
    Task<bool> While(ITestcontainersContainer testcontainers, ILogger logger);
  }
}
