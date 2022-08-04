namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Linq;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  internal class UntilHttpRequestIsCompleted : IWaitUntil, IDisposable
  {
    private readonly HttpWaitRequest request;
    private readonly int frequency;
    private readonly int timeout;
    private readonly HttpClientHandler handler;
    private readonly bool disposeHandler;

    public UntilHttpRequestIsCompleted(HttpWaitRequest request, int frequency, int timeout)
      : this(request, frequency, timeout, new HttpClientHandler(), true)
    {
    }

    public UntilHttpRequestIsCompleted(HttpWaitRequest request, int frequency, int timeout, HttpClientHandler handler, bool disposeHandler = false)
    {
      this.request = request;
      this.handler = handler;
      this.disposeHandler = disposeHandler;
      this.frequency = frequency;
      this.timeout = timeout;
    }

    public async Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
    {
      var httpClient = new HttpClient(this.handler, false);
      httpClient.Timeout = this.request.ReadTimeout;

      await WaitStrategy.WaitUntil(
        async () =>
        {
          try
          {
            var response = await httpClient.SendAsync(
              new HttpRequestMessage(
                this.request.Method,
                this.BuildRequestUri(testcontainers.Hostname, testcontainers.GetMappedPublicPort(this.request.Port))));

            if (this.request.StatusCodes.Any() && !this.request.StatusCodes.Contains((int)response.StatusCode))
            {
              return false;
            }

            return response.IsSuccessStatusCode;
          }
          catch (HttpRequestException)
          {
            return false;
          }
        },
        this.frequency,
        this.timeout,
        CancellationToken.None);

      return true;
    }

    public void Dispose()
    {
      if (this.disposeHandler)
      {
        this.handler?.Dispose();
        GC.SuppressFinalize(this);
      }
    }

    private Uri BuildRequestUri(string hostname, ushort port)
    {
      string portSuffix;
      if (port == 80)
      {
        portSuffix = string.Empty;
      }
      else
      {
        portSuffix = ":" + port;
      }

      var path = this.request.Path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? this.request.Path : "/" + this.request.Path;

      return new Uri($"http://{hostname}{portSuffix}{path}");
    }
  }
}
