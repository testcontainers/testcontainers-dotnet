namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Threading.Tasks;

  public interface IWaitUntil
  {
    Task<bool> Until(Uri endpoint, string id);
  }
}
