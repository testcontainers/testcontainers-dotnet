namespace Testcontainers.ActiveMq;

// # --8<-- [start:UseArtemisContainer]
public abstract class ArtemisContainerTest : IAsyncLifetime
{
    private readonly ArtemisContainer _artemisContainer;

    private readonly string _username;

    private readonly string _password;

    private ArtemisContainerTest(ArtemisContainer artemisContainer, string username, string password)
    {
        _artemisContainer = artemisContainer;
        _username = username;
        _password = password;
    }

    public async ValueTask InitializeAsync()
    {
        await _artemisContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }
    // # --8<-- [end:UseArtemisContainer]

    // # --8<-- [start:EstablishConnection]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesConnection()
    {
        // Given
        var connectionFactory = new ConnectionFactory(_artemisContainer.GetBrokerAddress());
        connectionFactory.UserName = _username;
        connectionFactory.Password = _password;

        // When
        using var connection = await connectionFactory.CreateConnectionAsync()
            .ConfigureAwait(true);

        await connection.StartAsync()
            .ConfigureAwait(true);

        Assert.True(connection.IsStarted);

        // Then
        using var session = await connection.CreateSessionAsync()
            .ConfigureAwait(true);

        using var queue = await session.CreateTemporaryQueueAsync()
            .ConfigureAwait(true);

        using var producer = await session.CreateProducerAsync(queue)
            .ConfigureAwait(true);

        using var consumer = await session.CreateConsumerAsync(queue)
            .ConfigureAwait(true);

        var producedMessage = await producer.CreateTextMessageAsync(Guid.NewGuid().ToString("D"))
            .ConfigureAwait(true);

        await producer.SendAsync(producedMessage)
            .ConfigureAwait(true);

        var receivedMessage = await consumer.ReceiveAsync()
            .ConfigureAwait(true);

        Assert.Equal(producedMessage.Text, receivedMessage.Body<string>());
    }
    // # --8<-- [end:EstablishConnection]

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _artemisContainer.DisposeAsync();
    }

    // # --8<-- [start:UseArtemisContainerDefaultAuth]
    [UsedImplicitly]
    public sealed class DefaultCredentialsConfiguration : ArtemisContainerTest
    {
        public DefaultCredentialsConfiguration()
            : base(new ArtemisBuilder(TestSession.GetImageFromDockerfile()).Build(), ArtemisBuilder.DefaultUsername, ArtemisBuilder.DefaultPassword)
        {
        }
    }
    // # --8<-- [end:UseArtemisContainerDefaultAuth]

    // # --8<-- [start:UseArtemisContainerCustomAuth]
    [UsedImplicitly]
    public sealed class CustomCredentialsConfiguration : ArtemisContainerTest
    {
        private static readonly string Username = Guid.NewGuid().ToString("D");

        private static readonly string Password = Guid.NewGuid().ToString("D");

        public CustomCredentialsConfiguration()
            : base(new ArtemisBuilder(TestSession.GetImageFromDockerfile()).WithUsername(Username).WithPassword(Password).Build(), Username, Password)
        {
        }
    }
    // # --8<-- [end:UseArtemisContainerCustomAuth]

    // # --8<-- [start:UseArtemisContainerNoAuth]
    [UsedImplicitly]
    public sealed class NoAuthCredentialsConfiguration : ArtemisContainerTest
    {
        public NoAuthCredentialsConfiguration()
            : base(new ArtemisBuilder(TestSession.GetImageFromDockerfile()).WithEnvironment("ANONYMOUS_LOGIN", bool.TrueString).Build(), string.Empty, string.Empty)
        {
        }
    }
    // # --8<-- [end:UseArtemisContainerNoAuth]
}