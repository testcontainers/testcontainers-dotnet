namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Net.Http;
  using JetBrains.Annotations;

  /// <summary>
  /// A http wait request.
  /// </summary>
  [PublicAPI]
  public class HttpWaitRequest
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpWaitRequest" /> class.
    /// </summary>
    /// <param name="port">The private container port.</param>
    /// <param name="statusCodes">The expected status codes.</param>
    /// <param name="path">The absolute path of the request uri.</param>
    /// <param name="method">The http method.</param>
    /// <param name="readTimeout">the http connection read timeout.</param>
    private HttpWaitRequest(int port, ISet<int> statusCodes, string path, HttpMethod method, TimeSpan readTimeout)
    {
      this.Port = port;
      this.StatusCodes = statusCodes;
      this.Path = path;
      this.Method = method;
      this.ReadTimeout = readTimeout;
    }

    public int Port { get; }

    public string Path { get; }

    public HttpMethod Method { get; }

    public ISet<int> StatusCodes { get; }

    public TimeSpan ReadTimeout { get; }

    /// <summary>
    /// Returns a new instance of wait request builder for a given port
    /// </summary>
    /// <param name="port">The private container port.</param>
    /// <returns>Instance of <see cref="HttpWaitRequest.Builder" />.</returns>
    [PublicAPI]
    public static Builder ForPort(int port)
    {
      return new Builder(port);
    }

    /// <summary>
    /// A fluent wait request builder.
    /// </summary>
    [SuppressMessage("ReSharper", "ParameterHidesMember", Justification = "Fluent builder")]
    [PublicAPI]
    public class Builder
    {
      private const string DefaultPath = "/";

      private readonly int port;
      private readonly ISet<int> statusCodes = new HashSet<int>();
      private string path = DefaultPath;
      private HttpMethod method = HttpMethod.Get;
      private TimeSpan readTimeout = TimeSpan.FromSeconds(1);

      internal Builder(int port)
      {
        this.port = port;
      }

      /// <summary>
      /// Waits for the given status code.
      /// </summary>
      /// <param name="statusCode">The expected status code.</param>
      /// <returns>A configured instance of <see cref="Builder" />.</returns>
      [PublicAPI]
      public Builder ForStatusCode(int statusCode)
      {
        this.statusCodes.Add(statusCode);
        return this;
      }

      /// <summary>
      /// Wait for the given path.
      /// </summary>
      /// <param name="path">The absolute path of the request uri.</param>
      /// <returns>A configured instance of <see cref="Builder" />.</returns>
      [PublicAPI]
      public Builder ForPath(string path)
      {
        this.path = string.IsNullOrWhiteSpace(path) ? DefaultPath : path.Trim();
        return this;
      }

      /// <summary>
      ///  Indicates the HTTP method to use (<see cref="HttpMethod.Get" /> by default).
      /// </summary>
      /// <param name="method">The http method.</param>
      /// <returns>A configured instance of <see cref="Builder" />.</returns>
      [PublicAPI]
      public Builder WithMethod(HttpMethod method)
      {
        this.method = method;
        return this;
      }

      /// <summary>
      /// Set the HTTP connections read timeout.
      /// </summary>
      /// <param name="timeout">The timeout.</param>
      /// <returns>A configured instance of <see cref="Builder" />.</returns>
      [PublicAPI]
      public Builder WithReadTimeout(TimeSpan timeout)
      {
        this.readTimeout = timeout;
        return this;
      }

      /// <summary>
      /// Builds the instance of <see cref="HttpWaitRequest" /> with the given configuration.
      /// </summary>
      /// <returns>A configured instance of <see cref="HttpWaitRequest" />.</returns>
      [PublicAPI]
      public HttpWaitRequest Build()
      {
        return new HttpWaitRequest(
          this.port,
          this.statusCodes,
          this.path,
          this.method,
          this.readTimeout);
      }
    }
  }
}
