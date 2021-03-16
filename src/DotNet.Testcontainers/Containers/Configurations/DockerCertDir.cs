namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.IO;
  using System.Linq;
  using static DockerClientConstants;

  public record DockerCertDir
  {
    private static readonly string[] RequiredCertificateFiles =
    {
      DockerClientCertFileName, DockerClientKeyFileName
    };

    public DockerCertDir(string certificatesDirectory)
    {
      var files = Directory.GetFiles(certificatesDirectory, "*.pem", SearchOption.TopDirectoryOnly);
      this.ClientAuthPossible = Directory.Exists(certificatesDirectory) && RequiredCertificateFiles.Length.Equals(
        files
          .Select(Path.GetFileName)
          .Intersect(RequiredCertificateFiles, StringComparer.CurrentCultureIgnoreCase)
          .Count());

      this.TlsVerifyPossible = files.Select(Path.GetFileName).Any(file => string.Equals(file, DockerCaFileName, StringComparison.InvariantCultureIgnoreCase));

      if (this.ClientAuthPossible)
      {
        this.PublicKeyFilePath = files.First(f => Path.GetFileName(f).Equals(DockerClientCertFileName, StringComparison.InvariantCultureIgnoreCase));
        this.PrivateKeyFilePath = files.First(f => Path.GetFileName(f).Equals(DockerClientKeyFileName, StringComparison.InvariantCultureIgnoreCase));
      }

      if (this.TlsVerifyPossible)
      {
        this.CaFilePath = files.First(f => Path.GetFileName(f).Equals(DockerCaFileName, StringComparison.InvariantCultureIgnoreCase));
      }
    }

    public string CaFilePath { get; } = string.Empty;

    public string PublicKeyFilePath { get; } = string.Empty;

    public string PrivateKeyFilePath { get; } = string.Empty;

    public bool ClientAuthPossible { get; }

    public bool TlsVerifyPossible { get; }
  }
}
