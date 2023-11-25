namespace Testcontainers.ActiveMq;

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

    public Task InitializeAsync()
    {
        return _artemisContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _artemisContainer.DisposeAsync().AsTask();
    }

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
            .ConfigureAwait(false);

        await connection.StartAsync()
            .ConfigureAwait(false);

        // Then
        Assert.True(connection.IsStarted);
    }

    [UsedImplicitly]
    public sealed class DefaultCredentialsConfiguration : ArtemisContainerTest
    {
        public DefaultCredentialsConfiguration()
            : base(new ArtemisBuilder().Build(), ArtemisBuilder.DefaultUsername, ArtemisBuilder.DefaultPassword)
        {
        }
    }

    [UsedImplicitly]
    public sealed class CustomCredentialsConfiguration : ArtemisContainerTest
    {
        private static readonly string Username = Guid.NewGuid().ToString("D");

        private static readonly string Password = Guid.NewGuid().ToString("D");

        public CustomCredentialsConfiguration()
            : base(new ArtemisBuilder().WithUsername(Username).WithPassword(Password).Build(), Username, Password)
        {
        }
    }

    [UsedImplicitly]
    public sealed class NoAuthCredentialsConfiguration : ArtemisContainerTest
    {
        public NoAuthCredentialsConfiguration()
            : base(new ArtemisBuilder().WithEnvironment("ANONYMOUS_LOGIN", bool.TrueString).Build(), string.Empty, string.Empty)
        {
        }
    }
}