namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Logging;

  public interface IWaitUntil
  {
    Task<bool> Until(Uri endpoint, string id, ILogger logger);
  }
}
