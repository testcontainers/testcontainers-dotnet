namespace DotNet.Testcontainers.Containers.WaitStrategies
{
  using System;
  using System.Threading.Tasks;
  using Configurations;

  public interface IWaitUntil
  {
    Task<bool> Until(Uri endpoint, DockerClientAuthConfig clientAuthConfig, string id);
  }
}
