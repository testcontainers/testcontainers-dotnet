namespace Testcontainers.PostgreSql.Examples;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Npgsql;

/// <summary>
/// Example demonstrating how to use PostgreSQL with SSL configuration using client certificate authentication.
/// This example shows how to set up a PostgreSQL container with server-side SSL certificates
/// and connect to it using client certificates for mutual TLS authentication.
/// </summary>
public static class PostgreSqlSSLConfigExample
{
    /// <summary>
    /// Demonstrates creating a PostgreSQL container with SSL configuration and client certificate authentication.
    /// </summary>
    public static async Task RunExample()
    {
        // Create temporary directory for SSL certificates
        var tempDir = Path.Combine(Path.GetTempPath(), "testcontainers-ssl-example");
        Directory.CreateDirectory(tempDir);

        try
        {
            // Generate SSL certificates for the example
            var (caCertPath, serverCertPath, serverKeyPath, clientCertPath, clientKeyPath) =
                await GenerateSSLCertificates(tempDir);

            Console.WriteLine("Generated SSL certificates for PostgreSQL server configuration:");
            Console.WriteLine($"  CA Certificate: {caCertPath}");
            Console.WriteLine($"  Server Certificate: {serverCertPath}");
            Console.WriteLine($"  Server Private Key: {serverKeyPath}");
            Console.WriteLine();

            // Create PostgreSQL container with SSL configuration
            await using var container = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .WithDatabase("example_db")
                .WithUsername("ssl_user")
                .WithPassword("secure_password123")
                .WithSSLConfig(caCertPath, serverCertPath, serverKeyPath)
                .Build();

            Console.WriteLine("Starting PostgreSQL container with SSL configuration...");
            await container.StartAsync();
            Console.WriteLine("PostgreSQL container started successfully with SSL enabled!");
            Console.WriteLine();

            // Example 1: Connect with SSL required but without client certificate
            await ConnectWithSSLOnly(container);

            // Example 2: Connect with client certificate authentication
            await ConnectWithClientCertificate(container, caCertPath, clientCertPath, clientKeyPath);

            // Example 3: Demonstrate SSL connection properties
            await DemonstrateSSLProperties(container);

            Console.WriteLine("SSL configuration example completed successfully!");
        }
        finally
        {
            // Clean up temporary certificates
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
                Console.WriteLine($"Cleaned up temporary directory: {tempDir}");
            }
        }
    }

    /// <summary>
    /// Connects to PostgreSQL with SSL required but without client certificate.
    /// </summary>
    private static async Task ConnectWithSSLOnly(PostgreSqlContainer container)
    {
        Console.WriteLine("Example 1: Connecting with SSL required (no client certificate)");

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(container.GetConnectionString())
        {
            SslMode = SslMode.Require,
            TrustServerCertificate = true // For demo only - use proper certificate validation in production
        };

        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        Console.WriteLine($"  ✓ Connected successfully with SSL");

        // Ensure sslinfo extension is available for SSL inspection functions
        await using (var enableExt = new NpgsqlCommand("CREATE EXTENSION IF NOT EXISTS sslinfo;", connection))
        {
            await enableExt.ExecuteNonQueryAsync();
        }

        // Verify SSL is active
        await using var command = new NpgsqlCommand("SELECT ssl_is_used();", connection);
        var sslIsUsed = await command.ExecuteScalarAsync();
        Console.WriteLine($"  ✓ SSL is active: {sslIsUsed}");
        Console.WriteLine();
    }

    /// <summary>
    /// Connects to PostgreSQL using client certificate authentication.
    /// </summary>
    private static async Task ConnectWithClientCertificate(PostgreSqlContainer container,
        string caCertPath, string clientCertPath, string clientKeyPath)
    {
        Console.WriteLine("Example 2: Connecting with client certificate authentication");

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(container.GetConnectionString())
        {
            // Validate server certificate against our generated CA (no hostname verification to avoid IP/hostname mismatch)
            SslMode = SslMode.VerifyCA,
            RootCertificate = caCertPath,
            ClientCertificate = clientCertPath,
            ClientCertificateKey = clientKeyPath
        };

        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        Console.WriteLine($"  ✓ Connected successfully with client certificate");

        // Create a test table and insert data
        await using var createCommand = new NpgsqlCommand(
            "CREATE TABLE IF NOT EXISTS ssl_test (id SERIAL PRIMARY KEY, message TEXT, created_at TIMESTAMP DEFAULT NOW());",
            connection);
        await createCommand.ExecuteNonQueryAsync();

        await using var insertCommand = new NpgsqlCommand(
            "INSERT INTO ssl_test (message) VALUES (@message) RETURNING id;",
            connection);
        insertCommand.Parameters.AddWithValue("@message", "SSL connection with client certificate successful!");

        var insertedId = await insertCommand.ExecuteScalarAsync();
        Console.WriteLine($"  ✓ Inserted record with ID: {insertedId}");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates various SSL connection properties and queries.
    /// </summary>
    private static async Task DemonstrateSSLProperties(PostgreSqlContainer container)
    {
        Console.WriteLine("Example 3: Demonstrating SSL connection properties");

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(container.GetConnectionString())
        {
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };

        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        // Ensure sslinfo extension is available for SSL inspection functions
        await using (var enableExt = new NpgsqlCommand("CREATE EXTENSION IF NOT EXISTS sslinfo;", connection))
        {
            await enableExt.ExecuteNonQueryAsync();
        }

        // Query SSL-related information
        var sslQueries = new[]
        {
            ("SSL Version", "SELECT ssl_version();"),
            ("SSL Cipher", "SELECT ssl_cipher();"),
            ("SSL Client Certificate Present", "SELECT CASE WHEN ssl_client_cert_present() THEN 'Yes' ELSE 'No' END;"),
            ("SSL Client Serial Number", "SELECT COALESCE(ssl_client_serial(), 'Not Available');")
        };

        foreach (var (description, query) in sslQueries)
        {
            try
            {
                await using var command = new NpgsqlCommand(query, connection);
                var result = await command.ExecuteScalarAsync();
                Console.WriteLine($"  {description}: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {description}: Error - {ex.Message}");
            }
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Generates SSL certificates for testing purposes.
    /// In production, use proper certificates from a trusted CA.
    /// </summary>
    private static async Task<(string CaCert, string ServerCert, string ServerKey, string ClientCert, string ClientKey)>
        GenerateSSLCertificates(string outputDir)
    {
        // Create CA certificate
        using var caRsa = RSA.Create(2048);
        var caCertRequest = new CertificateRequest(
            "CN=Test CA, O=Testcontainers Example",
            caRsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        caCertRequest.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(true, false, 0, true));

        caCertRequest.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                true));

        using var caCert = caCertRequest.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(365));

        var caCertPath = Path.Combine(outputDir, "ca_cert.pem");
        await File.WriteAllTextAsync(caCertPath,
            "-----BEGIN CERTIFICATE-----\n" +
            Convert.ToBase64String(caCert.RawData, Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END CERTIFICATE-----\n");

        // Create server certificate
        using var serverRsa = RSA.Create(2048);
        var serverCertRequest = new CertificateRequest(
            "CN=localhost, O=Testcontainers Example",
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

        var sanBuilder = new SubjectAlternativeNameBuilder();
        sanBuilder.AddDnsName("localhost");
        sanBuilder.AddIpAddress(System.Net.IPAddress.Loopback);
        serverCertRequest.CertificateExtensions.Add(sanBuilder.Build());

        using var serverCert = serverCertRequest.Create(
            caCert,
            caCert.NotBefore,
            caCert.NotAfter.AddSeconds(-5),
            new ReadOnlySpan<byte>(RandomNumberGenerator.GetBytes(16)));

        var serverCertPath = Path.Combine(outputDir, "server.crt");
        await File.WriteAllTextAsync(serverCertPath,
            "-----BEGIN CERTIFICATE-----\n" +
            Convert.ToBase64String(serverCert.RawData, Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END CERTIFICATE-----\n");

        var serverKeyPath = Path.Combine(outputDir, "server.key");
        await File.WriteAllTextAsync(serverKeyPath,
            "-----BEGIN PRIVATE KEY-----\n" +
            Convert.ToBase64String(serverRsa.ExportPkcs8PrivateKey(), Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END PRIVATE KEY-----\n");

        // Create client certificate
        using var clientRsa = RSA.Create(2048);
        var clientCertRequest = new CertificateRequest(
            "CN=testcontainers-client, O=Testcontainers Example",
            clientRsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        clientCertRequest.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                true));

        clientCertRequest.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(
                new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") }, // Client Authentication
                true));

        using var clientCert = clientCertRequest.Create(
            caCert,
            caCert.NotBefore,
            caCert.NotAfter.AddSeconds(-5),
            new ReadOnlySpan<byte>(RandomNumberGenerator.GetBytes(16)));

        var clientCertPath = Path.Combine(outputDir, "client.crt");
        await File.WriteAllTextAsync(clientCertPath,
            "-----BEGIN CERTIFICATE-----\n" +
            Convert.ToBase64String(clientCert.RawData, Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END CERTIFICATE-----\n");

        var clientKeyPath = Path.Combine(outputDir, "client.key");
        await File.WriteAllTextAsync(clientKeyPath,
            "-----BEGIN PRIVATE KEY-----\n" +
            Convert.ToBase64String(clientRsa.ExportPkcs8PrivateKey(), Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END PRIVATE KEY-----\n");

        // Set appropriate file permissions for private keys
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            File.SetUnixFileMode(serverKeyPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            File.SetUnixFileMode(clientKeyPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        }

        return (caCertPath, serverCertPath, serverKeyPath, clientCertPath, clientKeyPath);
    }

    /// <summary>
    /// Entry point for the example.
    /// </summary>
    public static async Task Main(string[] args)
    {
        Console.WriteLine("PostgreSQL SSL Configuration Example");
        Console.WriteLine("=====================================");
        Console.WriteLine();

        try
        {
            await RunExample();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Example failed with error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Environment.Exit(1);
        }
    }
}
