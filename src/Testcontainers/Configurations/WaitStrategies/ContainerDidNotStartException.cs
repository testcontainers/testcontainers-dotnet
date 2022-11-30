namespace DotNet.Testcontainers.Configurations.WaitStrategies
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// 
  /// </summary>
  public class ContainerDidNotStartException : System.Exception
  {
    public ContainerDidNotStartException(string message)
      : base(message)
    {
    }
  }
}
