namespace Testcontainers.Mosquitto;

public abstract class MosquittoContainerTest : ContainerTest<MosquittoBuilder, MosquittoContainer>
{
    private static readonly string Certificate = File.ReadAllText(Certificates.Instance.GetFilePath("server", "server.crt"));

    private static readonly string CertificateKey = File.ReadAllText(Certificates.Instance.GetFilePath("server", "server.key"));

    private readonly MqttClientFactory _clientFactory = new MqttClientFactory();

    private MosquittoContainerTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    protected abstract MqttClientOptions GetClientOptions();

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesConnection()
    {
        // Given
        using var client = _clientFactory.CreateMqttClient();

        // When
        var result = await client.ConnectAsync(GetClientOptions(), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(MqttClientConnectResultCode.Success, result.ResultCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task SubTopicReturnsPubMessage()
    {
        // Given
        const string helloMosquitto = "Hello, Mosquitto!";

        const string topicId = "hello-topic";

        var messageReceived = new TaskCompletionSource<string>();

        using var client = _clientFactory.CreateMqttClient();

        // When
        _ = await client.ConnectAsync(GetClientOptions(), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        client.ApplicationMessageReceivedAsync += e =>
        {
            messageReceived.SetResult(e.ApplicationMessage.ConvertPayloadToString());
            return Task.CompletedTask;
        };

        _ = await client.SubscribeAsync(topicId, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await client.PublishStringAsync(topicId, helloMosquitto, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var completedTask = await Task.WhenAny(messageReceived.Task, Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken))
            .ConfigureAwait(true);

        // Then
        Assert.Equal(messageReceived.Task, completedTask);

        var message = await messageReceived.Task
            .ConfigureAwait(true);

        Assert.Equal(helloMosquitto, message);
    }

    [UsedImplicitly]
    public sealed class TcpUnencryptedUnauthenticatedConfiguration(ITestOutputHelper testOutputHelper)
        : MosquittoContainerTest(testOutputHelper)
    {
        protected override MqttClientOptions GetClientOptions()
        {
            return new MqttClientOptionsBuilder()
                .WithTcpServer(Container.Hostname, Container.MqttPort)
                .Build();
        }
    }

    [UsedImplicitly]
    public sealed class TcpEncryptedUnauthenticatedConfiguration(ITestOutputHelper testOutputHelper)
        : MosquittoContainerTest(testOutputHelper)
    {
        protected override MosquittoBuilder Configure(MosquittoBuilder builder)
        {
            return builder.WithImage(TestSession.GetImageFromDockerfile()).WithCertificate(Certificate, CertificateKey);
        }

        protected override MqttClientOptions GetClientOptions()
        {
            return new MqttClientOptionsBuilder()
                .WithTcpServer(Container.Hostname, Container.MqttTlsPort)
                .WithTlsOptions(options => options.WithCertificateValidationHandler(e =>
                    "CN=Test CA".Equals(e.Certificate.Issuer, StringComparison.Ordinal)))
                .Build();
        }
    }

    [UsedImplicitly]
    public sealed class WebSocketUnencryptedUnauthenticatedConfiguration(ITestOutputHelper testOutputHelper)
        : MosquittoContainerTest(testOutputHelper)
    {
        protected override MosquittoBuilder Configure(MosquittoBuilder builder)
        {
            return builder.WithImage(TestSession.GetImageFromDockerfile());
        }

        protected override MqttClientOptions GetClientOptions()
        {
            return new MqttClientOptionsBuilder()
                .WithWebSocketServer(options => options.WithUri(Container.GetWsConnectionString()))
                .Build();
        }
    }

    [UsedImplicitly]
    public sealed class WebSocketEncryptedUnauthenticatedConfiguration(ITestOutputHelper testOutputHelper)
        : MosquittoContainerTest(testOutputHelper)
    {
        protected override MosquittoBuilder Configure(MosquittoBuilder builder)
        {
            return builder.WithImage(TestSession.GetImageFromDockerfile()).WithCertificate(Certificate, CertificateKey);
        }

        protected override MqttClientOptions GetClientOptions()
        {
            return new MqttClientOptionsBuilder()
                .WithWebSocketServer(options => options.WithUri(Container.GetWssConnectionString()))
                .WithTlsOptions(options => options.WithCertificateValidationHandler(e =>
                    "CN=Test CA".Equals(e.Certificate.Issuer, StringComparison.Ordinal)))
                .Build();
        }
    }
}