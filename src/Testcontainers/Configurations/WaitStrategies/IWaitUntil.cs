namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  [PublicAPI]
  public interface IWaitUntil
  {
    [PublicAPI]
    Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger);
  }
}
