namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System.Threading.Tasks;

  public interface IWaitUntil
  {
    Task<bool> Until(string id);
  }
}
