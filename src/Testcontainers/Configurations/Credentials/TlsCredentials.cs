namespace DotNet.Testcontainers.Configurations.Credentials
{
  using System.Net;
  using System.Net.Http;
  using System.Net.Security;
  using Docker.DotNet;
  using JetBrains.Annotations;
  using Microsoft.Net.Http.Client;

  public class TlsCredentials : Credentials
  {
    public TlsCredentials([CanBeNull] RemoteCertificateValidationCallback serverCertificateValidationCallback)
    {
      this.ServerCertificateValidationCallback = serverCertificateValidationCallback;
    }

    public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }

    public override bool IsTlsCredentials()
    {
      return true;
    }

    public override HttpMessageHandler GetHandler(HttpMessageHandler innerHandler)
    {
      var handler = (ManagedHandler)innerHandler;
      handler.ServerCertificateValidationCallback = this.ServerCertificateValidationCallback ?? ServicePointManager.ServerCertificateValidationCallback;
      return handler;
    }
  }
}
