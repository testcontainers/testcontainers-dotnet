namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  public interface IWaitUntil
  {
    Task<bool> Until(ITestcontainersContainer container, ILogger logger);
  }
}
