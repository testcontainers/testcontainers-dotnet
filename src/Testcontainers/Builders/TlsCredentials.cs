namespace DotNet.Testcontainers.Builders
{
  using System.Net.Http;
  using Docker.DotNet.X509;
  using Microsoft.Net.Http.Client;

  internal sealed class TlsCredentials : CertificateCredentials
  {
    public TlsCredentials()
      : base(null)
    {
    }

    public override bool IsTlsCredentials()
    {
      return true;
    }

    public override HttpMessageHandler GetHandler(HttpMessageHandler handler)
    {
      var managedHandler = (ManagedHandler)handler;
      managedHandler.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
      return managedHandler;
    }
  }
}
