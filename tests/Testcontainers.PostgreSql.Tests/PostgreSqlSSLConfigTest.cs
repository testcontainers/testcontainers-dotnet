#nullable enable
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Testcontainers.PostgreSql.Tests;

[UsedImplicitly]
public sealed class PostgreSqlSslConfigTest
{
    private const string CaCertFileName = "ca_cert.pem";
    private const string ServerCertFileName = "server.crt";
    private const string ServerKeyFileName = "server.key";
    private const string ClientCertFileName = "client.crt";
    private const string ClientKeyFileName = "client.key";

    private readonly string _tempDir;
    private readonly string _caCertPath;
    private readonly string _serverCertPath;
    private readonly string _serverKeyPath;

    private PostgreSqlContainer? _postgreSqlContainer;

    public PostgreSqlSslConfigTest()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "testcontainers-ssl-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_tempDir);

        _caCertPath = Path.Combine(_tempDir, CaCertFileName);
        _serverCertPath = Path.Combine(_tempDir, ServerCertFileName);
        _serverKeyPath = Path.Combine(_tempDir, ServerKeyFileName);
        Path.Combine(_tempDir, ClientCertFileName);
        Path.Combine(_tempDir, ClientKeyFileName);
    }

    private async Task EnsureContainerStartedAsync()
    {
        if (_postgreSqlContainer != null)
        {
            return;
        }

        // Generate SSL certificates for testing
        await GenerateSSLCertificates();

        // Create and start the PostgreSQL container with SSL configuration
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

        await _postgreSqlContainer.StartAsync();
    }

    private void Cleanup()
    {
        try
        {
            _postgreSqlContainer?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        catch
        {
            // ignore
        }

        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Fact]
    public async Task PostgreSqlContainerCanStartWithSSLSettings()
    {
        // Given
        await EnsureContainerStartedAsync();
        Assert.NotNull(_postgreSqlContainer);

        // When
        var connectionString = _postgreSqlContainer!.GetConnectionString();

        // Then
        Assert.NotEmpty(connectionString);
        Assert.Contains("testdb", connectionString);
        Assert.Contains("testuser", connectionString);
    }

    [Fact]
    public async Task PostgreSqlContainerCanConnectWithSSL()
    {
        // Given
        await EnsureContainerStartedAsync();
        Assert.NotNull(_postgreSqlContainer);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_postgreSqlContainer!.GetConnectionString())
        {
            SslMode = Npgsql.SslMode.Require,
            TrustServerCertificate = true // For testing only - in production use proper certificate validation
        };

        // When
        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);

        // Verify SSL is being used
        await using var command =
            new NpgsqlCommand("SELECT ssl FROM pg_stat_ssl WHERE pid = pg_backend_pid();", connection);
        var sslIsUsed = await command.ExecuteScalarAsync(TestContext.Current.CancellationToken);
        Assert.True(sslIsUsed is bool b && b, "SSL should be enabled for the connection");
    }

    [Fact]
    public async Task PostgreSqlContainerWithSSLCanExecuteQueries()
    {
        // Given
        await EnsureContainerStartedAsync();
        Assert.NotNull(_postgreSqlContainer);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_postgreSqlContainer!.GetConnectionString())
        {
            SslMode = Npgsql.SslMode.Require,
            TrustServerCertificate = true
        };

        // When
        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        await using var command =
            new NpgsqlCommand("CREATE TABLE test_table (id SERIAL PRIMARY KEY, name VARCHAR(100));", connection);
        await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);

        await using var insertCommand =
            new NpgsqlCommand("INSERT INTO test_table (name) VALUES ('Test SSL Connection');", connection);
        await insertCommand.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);

        await using var selectCommand = new NpgsqlCommand("SELECT COUNT(*) FROM test_table;", connection);
        var count = await selectCommand.ExecuteScalarAsync(TestContext.Current.CancellationToken);

        // Then
        Assert.Equal(1L, count);
    }

    [Fact]
    public void WithSSLCSettingsThrowsArgumentExceptionForEmptyCaCert()
    {
        // Given, When, Then
        var exception = Assert.Throws<ArgumentException>(() =>
            new PostgreSqlBuilder().WithSSLSettings("", _serverCertPath, _serverKeyPath));

        Assert.Equal("caCertFile", exception.ParamName);
        Assert.Contains("CA certificate file path cannot be null or empty", exception.Message);
    }

    [Fact]
    public void WithSSLSettingsThrowsArgumentExceptionForEmptyServerCert()
    {
        // Given, When, Then
        var exception = Assert.Throws<ArgumentException>(() =>
            new PostgreSqlBuilder().WithSSLSettings(_caCertPath, "", _serverKeyPath));

        Assert.Equal("serverCertFile", exception.ParamName);
        Assert.Contains("Server certificate file path cannot be null or empty", exception.Message);
    }

    [Fact]
    public void WithSSLSettingsThrowsArgumentExceptionForEmptyServerKey()
    {
        // Given, When, Then
        var exception = Assert.Throws<ArgumentException>(() =>
            new PostgreSqlBuilder().WithSSLSettings(_caCertPath, _serverCertPath, ""));

        Assert.Equal("serverKeyFile", exception.ParamName);
        Assert.Contains("Server key file path cannot be null or empty", exception.Message);
    }

    private async Task GenerateSSLCertificates()
    {
        // Create a simple RSA key pair for testing
        using var rsa = RSA.Create(2048);

        // Create CA certificate
        var caCertRequest = new CertificateRequest(
            "CN=Test CA, O=Testcontainers",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        caCertRequest.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(true, false, 0, true));

        caCertRequest.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                true));

        var caNotBefore = DateTimeOffset.UtcNow.AddDays(-1);
        var caNotAfter = DateTimeOffset.UtcNow.AddDays(365);
        using var caCert = caCertRequest.CreateSelfSigned(caNotBefore, caNotAfter);

        // Save CA certificate
        await File.WriteAllTextAsync(_caCertPath,
            "-----BEGIN CERTIFICATE-----\n" +
            Convert.ToBase64String(caCert.RawData, Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END CERTIFICATE-----\n");

        // Create server certificate
        using var serverRsa = RSA.Create(2048);
        var serverCertRequest = new CertificateRequest(
            "CN=localhost, O=Testcontainers",
            serverRsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        serverCertRequest.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                true));

        serverCertRequest.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(
                new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, // Server Authentication
                true));

        // Add Subject Alternative Names
        var sanBuilder = new SubjectAlternativeNameBuilder();
        sanBuilder.AddDnsName("localhost");
        sanBuilder.AddIpAddress(IPAddress.Loopback);
        serverCertRequest.CertificateExtensions.Add(sanBuilder.Build());

        var serverNotBefore = caNotBefore.AddMinutes(1) > DateTimeOffset.UtcNow.AddDays(-1)
            ? caNotBefore.AddMinutes(1)
            : DateTimeOffset.UtcNow.AddDays(-1);
        var serverNotAfter = caNotAfter.AddMinutes(-1);
        using var serverCert = serverCertRequest.Create(
            caCert,
            serverNotBefore,
            serverNotAfter,
            new ReadOnlySpan<byte>(RandomNumberGenerator.GetBytes(16)));

        // Save server certificate
        await File.WriteAllTextAsync(_serverCertPath,
            "-----BEGIN CERTIFICATE-----\n" +
            Convert.ToBase64String(serverCert.RawData, Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END CERTIFICATE-----\n");

        // Save server private key
        await File.WriteAllTextAsync(_serverKeyPath,
            "-----BEGIN PRIVATE KEY-----\n" +
            Convert.ToBase64String(serverRsa.ExportPkcs8PrivateKey(), Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END PRIVATE KEY-----\n");

        // Set appropriate permissions for private key
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            File.SetUnixFileMode(_serverKeyPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        }
    }
}