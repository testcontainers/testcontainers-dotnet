namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Logging;

  public interface IWaitWhile
  {
    Task<bool> While(Uri endpoint, string id, ILogger logger);
  }
}
