namespace Testcontainers.Azurite.Tests.Fixtures
{
  [UsedImplicitly]
  public sealed class AzuriteWithHttpsByPemFilesFixture : AzuriteDefaultFixture, IDisposable
  {
    private const string CertFileContent =
      "-----BEGIN CERTIFICATE-----\n" +
      "MIIDLTCCAhWgAwIBAgIIPo5isQA0jMMwDQYJKoZIhvcNAQELBQAwFDESMBAGA1UE\n" +
      "AxMJbG9jYWxob3N0MCAXDTIzMDIxNzE1MjcwMFoYDzIxMjMwMjE4MTUyNzAwWjAU\n" +
      "MRIwEAYDVQQDEwlsb2NhbGhvc3QwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEK\n" +
      "AoIBAQC8edYrVW0+q1aiv7gqnQbNkaF520n29ppwIgu0NqrEiauUn9IySu5aAQaV\n" +
      "sSyBWqNroik9vSSYsTF3ol0Y2Q5PB/CX7K32Asu7g/1l95MXRybRmN4ln2CPOt3C\n" +
      "t/ok7rOfFIBYAIG8+8Jh/R19uPwC1437Ha2gD8Exx6ct1LgVc2F7RLWRWBVolhq1\n" +
      "1J2irtct5w6mUaN7kmiM8Z+olagsgejp99Dgb5r44r7LhxFRDhf8RUVuPtC/7GoI\n" +
      "rtlXAg9QMNmffUvcF2NbEYhTZDKzpCyTJknEYeSUHP2B21/HMY6KiORl8pa6++/r\n" +
      "nvesK1IEiEDHONJuezUW2B9OMWIdAgMBAAGjgYAwfjAMBgNVHRMBAf8EAjAAMB0G\n" +
      "A1UdDgQWBBTZeQ/bxPzYv628wN4ixH/9Lb+AfDALBgNVHQ8EBAMCA+gwEwYDVR0l\n" +
      "BAwwCgYIKwYBBQUHAwEwGgYDVR0RBBMwEYIJbG9jYWxob3N0hwR/AAABMBEGCWCG\n" +
      "SAGG+EIBAQQEAwIGQDANBgkqhkiG9w0BAQsFAAOCAQEALx6P58sr0Cm0EUFVW0m2\n" +
      "EEv8CTYJsY4Au/rTkEvKR0S9XmezwXyUk+jC4rwPoPBBAZP7KkK40HZ2iIQx6iae\n" +
      "BHqmaGZqQr4SEJhzA1FI/48oSipHOoLp2h4ho5xG0BKwlCOOeOFOdaKsYZnxbHM3\n" +
      "NjKXy0KieI8CG57afZa6zQa71p35d/BU69kpf8hoH86t3hedSlFFGwxcBPg1IuXn\n" +
      "M1vpjrxeib2/oRa9K/9gOx2zQ9P0LMf9WMo2fOjm+Jsy46Q4Cb02yIH39zB4oVow\n" +
      "Y0fmW1SKj0r0GYwpLmtLuljZiangifER+ayZKINS32lmyswoKMRVUVP28tHnDHIk\n" +
      "gw==\n" +
      "-----END CERTIFICATE-----\n";

    private const string KeyFileContent =
      "-----BEGIN RSA PRIVATE KEY-----\n" +
      "MIIEpAIBAAKCAQEAvHnWK1VtPqtWor+4Kp0GzZGhedtJ9vaacCILtDaqxImrlJ/S\n" +
      "MkruWgEGlbEsgVqja6IpPb0kmLExd6JdGNkOTwfwl+yt9gLLu4P9ZfeTF0cm0Zje\n" +
      "JZ9gjzrdwrf6JO6znxSAWACBvPvCYf0dfbj8AteN+x2toA/BMcenLdS4FXNhe0S1\n" +
      "kVgVaJYatdSdoq7XLecOplGje5JojPGfqJWoLIHo6ffQ4G+a+OK+y4cRUQ4X/EVF\n" +
      "bj7Qv+xqCK7ZVwIPUDDZn31L3BdjWxGIU2Qys6QskyZJxGHklBz9gdtfxzGOiojk\n" +
      "ZfKWuvvv6573rCtSBIhAxzjSbns1FtgfTjFiHQIDAQABAoIBACykYyEie9zDMqMb\n" +
      "7CBTzz+zxd6aHVvcr1nuBn2qESq6PSTX8i6tZuV0pr2gGJ2O/XRKFaClA10TY1cl\n" +
      "4w02pFf91nP1wIKryNvieIvFZ1a1KLGulresl291jv9HGn3S+EKu1XOCszgzHaie\n" +
      "DnUv8qktq1iWgACQmr8SvjtxziRuJeHdfnSu+qtEwJvnWCZVP5/cp8PzjydHsZjj\n" +
      "KhDKtalYL3H1gMSMmvhQD8b02MKc2fnM0rVtl0HoiBh8Z05rp5HR/g6TCtEB6Nke\n" +
      "Yx+AK/YxsGZa6Rw25v8QQR1DoXhrcsHED8UB3MzMS/Sqh8sFVIV6wDJuRbZqHVEb\n" +
      "NClfFcECgYEA+0Y1D5LO/r6gv2S/I5ceMLPs+TzE1Tq4bngMTIl3PMB3j/LiqZUK\n" +
      "cIR8hpC9JpsTIwZReOM1XWcSEZznU/7HPRY+P/3I8F8jhvJFU++2CBKltaATQlql\n" +
      "Tzp13hXo6W4lebovJClNv4IxeCLmtmWx3BzNkjLJz35Y5y5LcxhwOHECgYEAwAVH\n" +
      "UacsCWrBZL04cCwH0dkDgauHHUR7bqbNKkn3xa4TVsT3IG7hCuhRkhNt6iBGgw8B\n" +
      "qPujE1dF79jdC8EqtYGtVlHiHYL5/414oy6jaAVzUJKcCLKDniQNAVlMnC3KeX3t\n" +
      "o6fcu+8J4eRaDAww3zDqKrFrEU6zNFe6SMF7+m0CgYBXtGKy4+BCRJFhHK4mowmT\n" +
      "oEm3mQFvF8bmMBrC4DxxRRC2euWooW/6ZBP++Cg9gGVGlV78nfmzd1V0Nlr8E3LB\n" +
      "nLDvrwpi9CI3aPrP2FymJrQWWCBAydjndZVMhkM8rwh/m3/21D/h93SC5VO4GHjj\n" +
      "Rl5uiDGurgAj+SG2s7H/0QKBgQCPyOQYnmd5PblG5/HE/RN17VyU0o5AOjF7L+fd\n" +
      "TLn+ClVs2dx7KsvU0RWTnnzlnflu1ePWV2dLakRyTx9mV+TVOR9EzdfVZWgyFgtJ\n" +
      "lfjCQaKRqNayJIot9qzOX8HgCOHei49Qxreg+mOaBNXww4gs+IHAKk4UDaxe+3oA\n" +
      "VPGzGQKBgQDZTaLwBCiLCvCxZNMVKdQ+W7ZRrsyE+iZptFMhsp6rieji+Hx0jJf3\n" +
      "m2tUiwmU2Vs/cIHi014Ve3FR7WX1i665iffVkKZP0Pvj/xLODMrqgTGUlnaIr0wi\n" +
      "caAU3hOCiEoVDowuD880pfxv1rGAwPouRRthpDoWa8zUPkzPbEsneQ==\n" +
      "-----END RSA PRIVATE KEY-----\n";

    public AzuriteWithHttpsByPemFilesFixture()
      : base(Setup)
    {
    }

    public static HttpClientTransport HttpClientTransport
    {
      get
      {
        var expectedCertificate = X509Certificate2.CreateFromPem(CertFileContent);
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

      const string certFileName = "certificate.pem";
      var certFilePath = Path.Combine(workspaceLocation, certFileName);
      File.WriteAllText(certFilePath, CertFileContent);

      const string keyFileName = "key.pem";
      var keyFilePath = Path.Combine(workspaceLocation, keyFileName);
      File.WriteAllText(keyFilePath, KeyFileContent);

      return builder
        .WithBindMount(workspaceLocation, AzuriteBuilder.DefaultWorkspaceDirectoryPath)
        .WithHttpsDefinedByPemFiles(
          Path.Combine(AzuriteBuilder.DefaultWorkspaceDirectoryPath, certFileName),
          Path.Combine(AzuriteBuilder.DefaultWorkspaceDirectoryPath, keyFileName));
    }
  }
}
