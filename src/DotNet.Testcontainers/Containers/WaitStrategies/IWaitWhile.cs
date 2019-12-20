namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Threading.Tasks;

  public interface IWaitWhile
  {
    Task<bool> While(Uri endpoint, string id);
  }
}
