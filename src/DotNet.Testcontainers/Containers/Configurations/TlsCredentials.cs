namespace DotNet.Testcontainers.Containers.Configurations
{
  using System.Net;
  using System.Net.Http;
  using System.Net.Security;
  using System.Security.Cryptography.X509Certificates;
  using Docker.DotNet;
  using Microsoft.Net.Http.Client;

  internal sealed class TlsCredentials : Credentials
  {
    private readonly bool isTls;
    private readonly RemoteCertificateValidationCallback serverCertificateValidationCallback;
    private readonly X509CertificateCollection clientCertificates;

    internal TlsCredentials(X509Certificate2 certificateAuthority, X509Certificate2 clientCertificate, bool isTls, bool tlsVerify)
    {
      this.isTls = isTls;
      this.clientCertificates = clientCertificate == null
        ? new X509CertificateCollection()
        : new X509Certificate2Collection
        {
          clientCertificate
        };

      this.serverCertificateValidationCallback = (certificateAuthority, tlsVerify) switch
      {
        (null, true) => ServicePointManager.ServerCertificateValidationCallback,
        ({ }, true) => RemoteCertificateValidation.CustomCaValidationCallback(certificateAuthority),
        (_, false) => RemoteCertificateValidation.IgnoreAllCallback()
      };
    }

    public override bool IsTlsCredentials()
    {
      return this.isTls;
    }

    public override HttpMessageHandler GetHandler(HttpMessageHandler innerHandler)
    {
      if (innerHandler is not ManagedHandler managedHandler)
      {
        return innerHandler;
      }

      managedHandler.ClientCertificates = this.clientCertificates;
      managedHandler.ServerCertificateValidationCallback = this.serverCertificateValidationCallback ?? ServicePointManager.ServerCertificateValidationCallback;

      return managedHandler;

    }
  }
}
