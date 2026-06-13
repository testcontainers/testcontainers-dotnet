namespace Testcontainers.PostgreSql;

public abstract class PostgreSqlContainerTest(PostgreSqlContainerTest.PostgreSqlDefaultFixture fixture)
{
    private static readonly string ServerCertificateFilePath = Certificates.Instance.GetFilePath("server", "server.crt");

    private static readonly string ServerCertificateKeyFilePath = Certificates.Instance.GetFilePath("server", "server.key");

    private static readonly string CaCertificateFilePath = Certificates.Instance.GetFilePath("ca", "ca.crt");

    // # --8<-- [start:UsePostgreSqlContainer]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = fixture.CreateConnection();

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(fixture.Container.GetConnectionString(), fixture.Container.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1;";

        // When
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    // # --8<-- [end:UsePostgreSqlContainer]

    public sealed class ReuseContainerTest : IClassFixture<PostgreSqlDefaultFixture>, IDisposable
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        private readonly PostgreSqlDefaultFixture _fixture;

        public ReuseContainerTest(PostgreSqlDefaultFixture fixture)
        {
            _fixture = fixture;
        }

        public void Dispose()
        {
            _cts.Dispose();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task StopsAndStartsContainerSuccessful(int _)
        {
            await _fixture.Container.StopAsync(_cts.Token)
                .ConfigureAwait(true);

            await _fixture.Container.StartAsync(_cts.Token)
                .ConfigureAwait(true);

            Assert.False(_cts.IsCancellationRequested);
        }
    }

    public class PostgreSqlDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<PostgreSqlBuilder, PostgreSqlContainer>(messageSink)
    {
        protected override PostgreSqlBuilder Configure()
            => new PostgreSqlBuilder(TestSession.GetImageFromDockerfile());

        public override DbProviderFactory DbProviderFactory
            => NpgsqlFactory.Instance;
    }

    [UsedImplicitly]
    public class PostgreSqlWaitForDatabaseFixture(IMessageSink messageSink)
        : PostgreSqlDefaultFixture(messageSink)
    {
        protected override PostgreSqlBuilder Configure()
            => base.Configure().WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public class PostgreSqlSslRequireFixture(IMessageSink messageSink)
        : PostgreSqlDefaultFixture(messageSink)
    {
        protected override PostgreSqlBuilder Configure()
            => base.Configure().WithSsl(ServerCertificateFilePath, ServerCertificateKeyFilePath);

        public override string ConnectionString
        {
            get
            {
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder(base.ConnectionString);
                connectionStringBuilder.TrustServerCertificate = true;
                connectionStringBuilder.SslMode = SslMode.Require;
                return connectionStringBuilder.ConnectionString;
            }
        }
    }

    [UsedImplicitly]
    public class PostgreSqlSslVerifyCaFixture(IMessageSink messageSink)
        : PostgreSqlDefaultFixture(messageSink)
    {
        // # --8<-- [start:PostgreSqlSslBuilder]
        protected override PostgreSqlBuilder Configure()
            => base.Configure().WithSsl(ServerCertificateFilePath, ServerCertificateKeyFilePath, CaCertificateFilePath);
        // # --8<-- [end:PostgreSqlSslBuilder]

        public override string ConnectionString
        {
            get
            {
                // # --8<-- [start:PostgreSqlSslConnectionString]
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder(base.ConnectionString);
                connectionStringBuilder.SslMode = SslMode.VerifyCA;
                connectionStringBuilder.RootCertificate = CaCertificateFilePath;
                return connectionStringBuilder.ConnectionString;
                // # --8<-- [end:PostgreSqlSslConnectionString]
            }
        }
    }

    [UsedImplicitly]
    public class PostgreSqlSslVerifyFullFixture(IMessageSink messageSink)
        : PostgreSqlDefaultFixture(messageSink)
    {
        protected override PostgreSqlBuilder Configure()
            => base.Configure().WithSsl(ServerCertificateFilePath, ServerCertificateKeyFilePath, CaCertificateFilePath);

        public override string ConnectionString
        {
            get
            {
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder(base.ConnectionString);
                // # --8<-- [start:PostgreSqlSslVerifyFull]
                // Npgsql checks VerifyFull against DNS SANs, it's necessary to use "localhost" instead of
                // the IP address. Testcontainers defaults to using the IP because of an old Docker bug
                // with IPv4/IPv6 port mapping, where "localhost" might resolve to a different public port.
                connectionStringBuilder.Host = "localhost";
                connectionStringBuilder.SslMode = SslMode.VerifyFull;
                // # --8<-- [end:PostgreSqlSslVerifyFull]
                connectionStringBuilder.RootCertificate = CaCertificateFilePath;
                return connectionStringBuilder.ConnectionString;
            }
        }
    }

    [UsedImplicitly]
    public sealed class PostgreSqlDefaultConfiguration(PostgreSqlDefaultFixture fixture)
        : PostgreSqlContainerTest(fixture), IClassFixture<PostgreSqlDefaultFixture>;

    [UsedImplicitly]
    public sealed class PostgreSqlWaitForDatabaseConfiguration(PostgreSqlWaitForDatabaseFixture fixture)
        : PostgreSqlContainerTest(fixture), IClassFixture<PostgreSqlWaitForDatabaseFixture>;

    [UsedImplicitly]
    public sealed class PostgreSqlSslRequireConfiguration(PostgreSqlSslRequireFixture fixture)
        : PostgreSqlContainerTest(fixture), IClassFixture<PostgreSqlSslRequireFixture>;

    [UsedImplicitly]
    public sealed class PostgreSqlSslVerifyCaConfiguration(PostgreSqlSslVerifyCaFixture fixture)
        : PostgreSqlContainerTest(fixture), IClassFixture<PostgreSqlSslVerifyCaFixture>;

    [UsedImplicitly]
    public sealed class PostgreSqlSslVerifyFullConfiguration(PostgreSqlSslVerifyFullFixture fixture)
        : PostgreSqlContainerTest(fixture), IClassFixture<PostgreSqlSslVerifyFullFixture>;
}