namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  [PublicAPI]
  public sealed class HttpWaitStrategy : IWaitUntil
  {
    private readonly IDictionary<string, string> httpHeaders = new Dictionary<string, string>();

    private readonly ISet<HttpStatusCode> httpStatusCodes = new HashSet<HttpStatusCode>();

    private Predicate<HttpStatusCode> httpStatusCodePredicate;

    private HttpMethod httpMethod;

    private string schemeName;

    private string pathValue;

    private ushort portNumber;

    public HttpWaitStrategy()
    {
      _ = this.WithMethod(HttpMethod.Get).UsingTls(false).ForPort(80).ForPath("/");
    }

    public async Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
    {
      string host;

      ushort port;

      try
      {
        host = testcontainers.Hostname;
        port = testcontainers.GetMappedPublicPort(this.portNumber);
      }
      catch
      {
        return false;
      }

      using (var httpClient = new HttpClient())
      {
        using (var httpRequestMessage = new HttpRequestMessage(this.httpMethod, new UriBuilder(this.schemeName, host, port, this.pathValue).Uri))
        {
          foreach (var httpHeader in this.httpHeaders)
          {
            httpRequestMessage.Headers.Add(httpHeader.Key, httpHeader.Value);
          }

          HttpResponseMessage httpResponseMessage;

          try
          {
            httpResponseMessage = await httpClient.SendAsync(httpRequestMessage)
              .ConfigureAwait(false);
          }
          catch
          {
            return false;
          }

          Predicate<HttpStatusCode> predicate;

          if (!this.httpStatusCodes.Any() && this.httpStatusCodePredicate == null)
          {
            predicate = statusCode => HttpStatusCode.OK.Equals(statusCode);
          }
          else if (this.httpStatusCodes.Any() && this.httpStatusCodePredicate == null)
          {
            predicate = statusCode => this.httpStatusCodes.Contains(statusCode);
          }
          else if (this.httpStatusCodes.Any())
          {
            predicate = statusCode => this.httpStatusCodes.Contains(statusCode) || this.httpStatusCodePredicate.Invoke(statusCode);
          }
          else
          {
            predicate = this.httpStatusCodePredicate;
          }

          return predicate.Invoke(httpResponseMessage.StatusCode);
        }
      }
    }

    public HttpWaitStrategy ForStatusCode(HttpStatusCode statusCode)
    {
      this.httpStatusCodes.Add(statusCode);
      return this;
    }

    public HttpWaitStrategy ForStatusCodeMatching(Predicate<HttpStatusCode> statusCodePredicate)
    {
      this.httpStatusCodePredicate = statusCodePredicate;
      return this;
    }

    public HttpWaitStrategy ForPath(string path)
    {
      this.pathValue = path;
      return this;
    }

    public HttpWaitStrategy ForPort(ushort port)
    {
      this.portNumber = port;
      return this;
    }

    public HttpWaitStrategy UsingTls(bool tlsEnabled = true)
    {
      this.schemeName = tlsEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
      return this;
    }

    public HttpWaitStrategy WithMethod(HttpMethod method)
    {
      this.httpMethod = method;
      return this;
    }

    public HttpWaitStrategy WithHeader(string name, string value)
    {
      this.httpHeaders.Add(name, value);
      return this;
    }

    public HttpWaitStrategy WithHeaders(IReadOnlyDictionary<string, string> headers)
    {
      foreach (var header in headers)
      {
        _ = this.WithHeader(header.Key, header.Value);
      }

      return this;
    }
  }
}
