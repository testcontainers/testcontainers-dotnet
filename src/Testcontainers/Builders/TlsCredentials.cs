namespace DotNet.Testcontainers.Builders
{
  using Docker.DotNet.X509;

  internal sealed class TlsCredentials : CertificateCredentials
  {
    public TlsCredentials()
      : base(null)
    {
    }
  }
}
