namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents the SSL certificate files a module uses to enable SSL/TLS.
  /// </summary>
  [PublicAPI]
  public sealed class SslCertificate
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SslCertificate" /> class.
    /// </summary>
    /// <param name="certificateFilePath">The SSL certificate file.</param>
    /// <param name="certificateKeyFilePath">The SSL certificate private key file.</param>
    /// <param name="caCertificateFilePath">The CA certificate file.</param>
    public SslCertificate(
      FilePath certificateFilePath,
      FilePath certificateKeyFilePath,
      FilePath? caCertificateFilePath = null)
    {
      CertificateFilePath = certificateFilePath;
      CertificateKeyFilePath = certificateKeyFilePath;
      CaCertificateFilePath = caCertificateFilePath;
    }

    /// <summary>
    /// Gets the SSL certificate file.
    /// </summary>
    public FilePath CertificateFilePath { get; }

    /// <summary>
    /// Gets the SSL certificate private key file.
    /// </summary>
    public FilePath CertificateKeyFilePath { get; }

    /// <summary>
    /// Gets the CA certificate file.
    /// </summary>
    public FilePath? CaCertificateFilePath { get; }
  }
}
