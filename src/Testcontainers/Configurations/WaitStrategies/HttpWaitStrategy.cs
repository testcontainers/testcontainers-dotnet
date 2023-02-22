namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// Wait for an HTTP(S) endpoint to return a particular status code.
  /// </summary>
  [PublicAPI]
  public sealed class HttpWaitStrategy : IWaitUntil
  {
    private const ushort HttpPort = 80;

    private const ushort HttpsPort = 443;

    private readonly IDictionary<string, string> httpHeaders = new Dictionary<string, string>();

    private readonly ISet<HttpStatusCode> httpStatusCodes = new HashSet<HttpStatusCode>();

    private Predicate<HttpStatusCode> httpStatusCodePredicate;

    private Func<HttpResponseMessage, Task<bool>> httpResponseMessagePredicate;

    private HttpMethod httpMethod;

    private string schemeName;

    private string pathValue;

    private ushort? portNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpWaitStrategy" /> class.
    /// </summary>
    public HttpWaitStrategy()
    {
      _ = this.WithMethod(HttpMethod.Get).UsingTls(false).ForPath("/").ForResponseMessageMatching(_ => Task.FromResult(true));
    }

    /// <inheritdoc />
    public async Task<bool> UntilAsync(IContainer container)
    {
      // Java falls back to the first exposed port. The .NET wait strategies do not have access to the exposed port information yet.
      var containerPort = this.portNumber.GetValueOrDefault(Uri.UriSchemeHttp.Equals(this.schemeName, StringComparison.OrdinalIgnoreCase) ? HttpPort : HttpsPort);

      string host;

      ushort port;

      try
      {
        host = container.Hostname;
        port = container.GetMappedPublicPort(containerPort);
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

          try
          {
            var responseMessagePredicate = await this.httpResponseMessagePredicate.Invoke(httpResponseMessage)
              .ConfigureAwait(false);

            return responseMessagePredicate && predicate.Invoke(httpResponseMessage.StatusCode);
          }
          catch
          {
            return false;
          }
          finally
          {
            httpResponseMessage.Dispose();
          }
        }
      }
    }

    /// <summary>
    /// Waits for the status code.
    /// </summary>
    /// <param name="statusCode">The expected status code.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy ForStatusCode(HttpStatusCode statusCode)
    {
      this.httpStatusCodes.Add(statusCode);
      return this;
    }

    /// <summary>
    /// Waits for the status code to pass the predicate.
    /// </summary>
    /// <param name="statusCodePredicate">The predicate to test the HTTP response against.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy ForStatusCodeMatching(Predicate<HttpStatusCode> statusCodePredicate)
    {
      this.httpStatusCodePredicate = statusCodePredicate;
      return this;
    }

    /// <summary>
    /// Waits for the response message to pass the predicate.
    /// </summary>
    /// <param name="responseMessagePredicate">The predicate to test the HTTP response against.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy ForResponseMessageMatching(Func<HttpResponseMessage, Task<bool>> responseMessagePredicate)
    {
      this.httpResponseMessagePredicate = responseMessagePredicate;
      return this;
    }

    /// <summary>
    /// Waits for the path.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy ForPath(string path)
    {
      this.pathValue = path;
      return this;
    }

    /// <summary>
    /// Waits for the port.
    /// </summary>
    /// <remarks>
    /// <see cref="HttpPort" /> default value.
    /// </remarks>
    /// <param name="port">The port to check.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy ForPort(ushort port)
    {
      this.portNumber = port;
      return this;
    }

    /// <summary>
    /// Indicates that the HTTP request use HTTPS.
    /// </summary>
    /// <remarks>
    /// <see cref="bool.FalseString" /> default value.
    /// </remarks>
    /// <param name="tlsEnabled">True if the HTTP request use HTTPS, otherwise false.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy UsingTls(bool tlsEnabled = true)
    {
      this.schemeName = tlsEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
      return this;
    }

    /// <summary>
    /// Indicates the HTTP request method.
    /// </summary>
    /// <remarks>
    /// <see cref="HttpMethod.Get" /> default value.
    /// </remarks>
    /// <param name="method">The HTTP method.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy WithMethod(HttpMethod method)
    {
      this.httpMethod = method;
      return this;
    }

    /// <summary>
    /// Adds a basic authentication HTTP header to the HTTP request.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy WithBasicAuthentication(string username, string password)
    {
      return this.WithHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(string.Join(":", username, password))));
    }

    /// <summary>
    /// Adds a custom HTTP header to the HTTP request.
    /// </summary>
    /// <param name="name">The HTTP header name.</param>
    /// <param name="value">The HTTP header value.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy WithHeader(string name, string value)
    {
      this.httpHeaders.Add(name, value);
      return this;
    }

    /// <summary>
    /// Adds custom HTTP headers to the HTTP request.
    /// </summary>
    /// <param name="headers">A list of HTTP headers.</param>
    /// <returns>A configured instance of <see cref="HttpWaitStrategy" />.</returns>
    public HttpWaitStrategy WithHeaders(IReadOnlyDictionary<string, string> headers)
    {
      return headers.Aggregate(this, (httpWaitStrategy, header) => httpWaitStrategy.WithHeader(header.Key, header.Value));
    }
  }
}
