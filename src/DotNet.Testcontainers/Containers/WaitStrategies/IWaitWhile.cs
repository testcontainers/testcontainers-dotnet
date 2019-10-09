namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System.Threading.Tasks;

  public interface IWaitWhile
  {
    Task<bool> While(string id);
  }
}
