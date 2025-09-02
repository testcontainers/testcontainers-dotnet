namespace TestContainers.Mosquitto.Tests;

public abstract class MosquittoContainerTest : ContainerTest<MosquittoBuilder, MosquittoContainer>
{
	private IMqttClient _client;

	protected MosquittoContainerTest(ITestOutputHelper outputHelper, Func<MosquittoBuilder, MosquittoBuilder> configure = null)
	  : base(outputHelper, configure)
	{
	}

	protected override async ValueTask InitializeAsync()
	{
		await base.InitializeAsync();

		var mqttFactory = new MqttClientFactory();
		_client = mqttFactory.CreateMqttClient();
	}

	protected override async ValueTask DisposeAsyncCore()
	{
		await _client?.TryDisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection);
		_client?.Dispose();

		await base.DisposeAsyncCore();
	}

	[Fact]
	[Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
	public async Task CanEstablishAConnection()
	{
		var result = await _client.ConnectAsync(GetClientOptions(), TestContext.Current.CancellationToken);
		Assert.Equal(MqttClientConnectResultCode.Success, result.ResultCode);
	}

	[Fact]
	[Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
	public async Task PublishedMessageIsReceived()
	{
		const string topic = "test/topic";
		const string payload = "Hello, MQTT!";

		var tcs = new TaskCompletionSource();

		var options = new MqttClientSubscribeOptionsBuilder()
		  .WithTopicFilter(f => f.WithTopic(topic))
		  .Build();

		bool messageReceived = false;

		await _client.ConnectAsync(GetClientOptions(), TestContext.Current.CancellationToken);
		_client.ApplicationMessageReceivedAsync += e =>
		{
			Assert.Equal(topic, e.ApplicationMessage.Topic);
			Assert.Equal(payload, e.ApplicationMessage.ConvertPayloadToString());
			messageReceived = true;
			tcs.SetResult();
			return Task.CompletedTask;
		};

		var sub = await _client.SubscribeAsync(options, TestContext.Current.CancellationToken);

		await _client.PublishStringAsync(topic, payload, cancellationToken: TestContext.Current.CancellationToken);
		await Task.WhenAny(tcs.Task, Task.Delay(-1, TestContext.Current.CancellationToken));

		Assert.True(messageReceived);
	}

	protected abstract MqttClientOptions GetClientOptions();

	[UsedImplicitly]
	public sealed class MosquittoTcpAnonymousConfiguration : MosquittoContainerTest
	{
		public MosquittoTcpAnonymousConfiguration(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		protected override MqttClientOptions GetClientOptions()
		{
			var builder = new MqttClientOptionsBuilder()
			  .WithClientId($"testcontainers.mosquitto-{Guid.NewGuid()}")
			  .WithCleanStart()
			  .WithTcpServer(Container.Hostname, Container.GetPort());

			return builder.Build();
		}
	}

	[UsedImplicitly]
	public sealed class MosquittoTcpEncryptedAnonymousConfiguration : MosquittoContainerTest
	{
		public MosquittoTcpEncryptedAnonymousConfiguration(ITestOutputHelper outputHelper)
			: base(outputHelper, builder => builder.WithCertificate(PemCertificate.Instance.Certificate, PemCertificate.Instance.CertificateKey))
		{
		}

		protected override MqttClientOptions GetClientOptions()
		{
			var builder = new MqttClientOptionsBuilder()
			  .WithTlsOptions(o =>
				o.UseTls()
				  .WithAllowUntrustedCertificates()
				  .WithIgnoreCertificateChainErrors()
				  .WithIgnoreCertificateRevocationErrors()
				  .WithCertificateValidationHandler(context =>
					{
						Assert.NotNull(context.Certificate);
						return true;
					})
			  )
			  .WithClientId($"testcontainers.mosquitto-{Guid.NewGuid()}")
			  .WithCleanStart()
			  .WithTcpServer(Container.Hostname, Container.GetSecurePort());

			return builder.Build();
		}
	}

	[UsedImplicitly]
	public sealed class MosquittoWebSocketAnonymousConfiguration : MosquittoContainerTest
	{
		public MosquittoWebSocketAnonymousConfiguration(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		protected override MqttClientOptions GetClientOptions()
		{
			var builder = new MqttClientOptionsBuilder()
			  .WithClientId($"testcontainers.mosquitto-{Guid.NewGuid()}")
			  .WithCleanStart()
			  .WithWebSocketServer(o => o.WithUri(Container.GetWsEndpoint()));

			return builder.Build();
		}
	}

	[UsedImplicitly]
	public sealed class MosquittoWebSocketSecureAnonymousConfiguration : MosquittoContainerTest
	{
		public MosquittoWebSocketSecureAnonymousConfiguration(ITestOutputHelper outputHelper)
			: base(outputHelper, builder => builder.WithCertificate(PemCertificate.Instance.Certificate, PemCertificate.Instance.CertificateKey))
		{
		}

		protected override MqttClientOptions GetClientOptions()
		{
			var builder = new MqttClientOptionsBuilder()
			  .WithTlsOptions(o =>
				o.UseTls()
				  .WithAllowUntrustedCertificates()
				  .WithIgnoreCertificateChainErrors()
				  .WithIgnoreCertificateRevocationErrors()
				  .WithCertificateValidationHandler(context =>
					{
						Assert.NotNull(context.Certificate);
						return true;
					})
			  )
			  .WithClientId($"testcontainers.mosquitto-{Guid.NewGuid()}")
			  .WithCleanStart()
			  .WithWebSocketServer(o => o.WithUri(Container.GetWssEndpoint()));

			return builder.Build();
		}
	}
}
