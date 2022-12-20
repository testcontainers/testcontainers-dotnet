namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  public sealed class HttpWaitStrategy : IWaitUntil
  {
    private readonly UriBuilder uriBuilder = new UriBuilder(Uri.UriSchemeHttp, "127.0.0.1");

    public Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
    {
      return Task.FromResult(false);
    }

    public HttpWaitStrategy ForPath(string path)
    {
      this.uriBuilder.Path = path;
      return this;
    }

    public HttpWaitStrategy UsingTls(bool tlsEnabled = true)
    {
      this.uriBuilder.Scheme = tlsEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
      return this;
    }
  }
}
