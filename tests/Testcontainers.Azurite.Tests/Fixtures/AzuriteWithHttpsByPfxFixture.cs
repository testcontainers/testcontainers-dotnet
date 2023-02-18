namespace Testcontainers.Azurite.Tests.Fixtures
{
  [UsedImplicitly]
  public sealed class AzuriteWithHttpsByPfxFixture : AzuriteDefaultFixture, IDisposable
  {
    private const string PfxFilePassword = "password";

    private const string PfxFileContent =
      "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";

    private static readonly byte[] PfxFileBytes = Convert.FromBase64String(PfxFileContent);

    public AzuriteWithHttpsByPfxFixture()
      : base(Setup)
    {
    }

    public static HttpClientTransport HttpClientTransport
    {
      get
      {
        var expectedCertificate = new X509Certificate2(PfxFileBytes, PfxFilePassword);
        var httpClientHandler = new HttpClientHandler
        {
          ServerCertificateCustomValidationCallback = (requestMessage, actualCertificate, chain, policyErrors) =>
          {
            return expectedCertificate.Thumbprint == actualCertificate.Thumbprint;
          },
        };
        return new HttpClientTransport(new HttpClient(httpClientHandler));
      }
    }

    public void Dispose()
    {
      var workspaceFolder = this.Container.WorkspaceLocationBinding;
      if (workspaceFolder != null && Directory.Exists(workspaceFolder))
      {
        Directory.Delete(workspaceFolder, true);
      }
    }

    private static AzuriteBuilder Setup(AzuriteBuilder builder)
    {
      var workspaceLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));
      Directory.CreateDirectory(workspaceLocation);

      const string pfxFileName = "localhost.pfx";
      File.WriteAllBytes(Path.Combine(workspaceLocation, pfxFileName), PfxFileBytes);

      return builder
        .WithBindMount(workspaceLocation, AzuriteBuilder.DefaultWorkspaceDirectoryPath)
        .WithHttpsDefinedByPfxFile(Path.Combine(AzuriteBuilder.DefaultWorkspaceDirectoryPath, pfxFileName), PfxFilePassword);
    }
  }
}
