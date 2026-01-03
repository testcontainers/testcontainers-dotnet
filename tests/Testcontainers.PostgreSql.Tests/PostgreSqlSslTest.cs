using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using Npgsql;
using Xunit;
using Xunit.Internal;

namespace Testcontainers.PostgreSql.Tests;

[UsedImplicitly]
public sealed class PostgreSqlSslTest : IAsyncLifetime
{
    private readonly string _tempDir;
    private readonly string _caCertPath;
    private readonly string _serverCertPath;
    private readonly string _serverKeyPath;

    private readonly PostgreSqlContainer _postgreSqlContainer;

    public PostgreSqlSslTest()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "testcontainers-ssl-" + Guid.NewGuid().ToString("N").Substring(0, 8));
        Directory.CreateDirectory(_tempDir);

        _caCertPath = Path.Combine(_tempDir, "ca_cert.pem");
        _serverCertPath = Path.Combine(_tempDir, "server.crt");
        _serverKeyPath = Path.Combine(_tempDir, "server.key");

        // Generate simple CA and server certificates for the test
        GenerateCertificatesAsync().GetAwaiter().GetResult();

        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass123")
            .WithSSLSettings(_caCertPath, _serverCertPath, _serverKeyPath)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilInternalTcpPortIsAvailable(PostgreSqlBuilder.PostgreSqlPort)
                .UntilMessageIsLogged("database system is ready to accept connections"))
            .Build();
    }

    [Fact]
    public async Task PostgreSqlContainerCanConnectWithSsl()
    {
        // Given
        await _postgreSqlContainer.StartAsync(TestContext.Current.CancellationToken);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_postgreSqlContainer.GetConnectionString())
        {
            SslMode = Npgsql.SslMode.Require,
            TrustServerCertificate = true
        };

        // When
        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    public ValueTask InitializeAsync()
    {
        // no-op, container started within test
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_postgreSqlContainer != null)
            {
                await _postgreSqlContainer.DisposeAsync();
            }
        }
        finally
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, recursive: true);
            }
        }
    }

    private static async Task WritePemAsync(string path, byte[] derBytes, string begin, string end)
    {
        await File.WriteAllTextAsync(path, $"{begin}\n{Convert.ToBase64String(derBytes, Base64FormattingOptions.InsertLineBreaks)}\n{end}\n");
    }

    private async Task GenerateCertificatesAsync()
    {
        using var caRsa = System.Security.Cryptography.RSA.Create(2048);
        var caReq = new System.Security.Cryptography.X509Certificates.CertificateRequest(
            "CN=Test CA, O=Testcontainers",
            caRsa,
            System.Security.Cryptography.HashAlgorithmName.SHA256,
            System.Security.Cryptography.RSASignaturePadding.Pkcs1);
        caReq.CertificateExtensions.Add(new System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension(true, false, 0, true));
        caReq.CertificateExtensions.Add(new System.Security.Cryptography.X509Certificates.X509KeyUsageExtension(
            System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.KeyCertSign |
            System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.CrlSign, true));
        var caNotBefore = DateTimeOffset.UtcNow.AddDays(-1);
        var caNotAfter = DateTimeOffset.UtcNow.AddDays(365);
        using var caCert = caReq.CreateSelfSigned(caNotBefore, caNotAfter);
        await WritePemAsync(_caCertPath, caCert.RawData, "-----BEGIN CERTIFICATE-----", "-----END CERTIFICATE-----");

        using var serverRsa = System.Security.Cryptography.RSA.Create(2048);
        var serverReq = new System.Security.Cryptography.X509Certificates.CertificateRequest(
            "CN=localhost, O=Testcontainers",
            serverRsa,
            System.Security.Cryptography.HashAlgorithmName.SHA256,
            System.Security.Cryptography.RSASignaturePadding.Pkcs1);
        serverReq.CertificateExtensions.Add(new System.Security.Cryptography.X509Certificates.X509KeyUsageExtension(
            System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.DigitalSignature |
            System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.KeyEncipherment, true));
        var san = new System.Security.Cryptography.X509Certificates.SubjectAlternativeNameBuilder();
        san.AddDnsName("localhost");
        san.AddIpAddress(System.Net.IPAddress.Loopback);
        serverReq.CertificateExtensions.Add(san.Build());
        var serverNotBefore = caNotBefore.AddMinutes(1);
        var serverNotAfter = caNotAfter.AddMinutes(-1);
        using var serverCert = serverReq.Create(
            caCert,
            serverNotBefore,
            serverNotAfter,
            new ReadOnlySpan<byte>(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16)));
        await WritePemAsync(_serverCertPath, serverCert.RawData, "-----BEGIN CERTIFICATE-----", "-----END CERTIFICATE-----");
        await File.WriteAllTextAsync(_serverKeyPath, "-----BEGIN PRIVATE KEY-----\n" +
            Convert.ToBase64String(serverRsa.ExportPkcs8PrivateKey(), Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END PRIVATE KEY-----\n");
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            File.SetUnixFileMode(_serverKeyPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        }
    }
}