#nullable enable
namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Net.Http;
  using System.Security;

  /// <summary>
  /// Configured the Request and Response Behaviour of the UntilHttp Wait
  /// </summary>
  public class UntilHttpOptions
  {
    public UntilHttpOptions()
    {
      this.Method = HttpMethod.Get;
      this.Path = "/";
      this.Host = "localhost";
      this.Port = 80;
      this.ExpectedResponseCodes = new() { HttpStatusCode.OK };
      this.TimeOut = TimeSpan.FromMinutes(1);
      this.RequestDelay = 1;
      this.MaxRetries = 10;
    }

    public HttpMethod Method { get; set; }
    public string Path { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public HashSet<HttpStatusCode> ExpectedResponseCodes { get; set; }
    public string? ExpectedOutput { get; set; }
    public HttpContent? RequestContent { get; set; }
    public bool UseSecure { get; set; }
    public SecureString? AuthString { get; set; }
    public bool UseAuth { get; set; }
    public TimeSpan TimeOut { get; set; }
    public bool ValidateContent { get; set; }
    public double RequestDelay { get; set; }
    public int MaxRetries { get; set; }

    public Uri Uri => new($"{(this.UseSecure ? "https" : "http")}://{this.Host}:{this.Port}{this.Path}");
  }
}
