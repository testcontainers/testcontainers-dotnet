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

    public override HttpMessageHandler GetHandler(HttpMessageHandler innerHandler)
    {
      var handler = (ManagedHandler)innerHandler;
      handler.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
      return handler;
    }
  }
}
