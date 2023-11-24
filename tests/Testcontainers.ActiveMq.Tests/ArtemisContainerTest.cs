namespace Testcontainers.ActiveMq;

public abstract class ArtemisContainerTest : IAsyncLifetime
{
    private readonly ArtemisContainer _artemisContainer;

    protected ArtemisContainerTest(ArtemisContainer artemisContainer)
    {
        _artemisContainer = artemisContainer;
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
    public abstract Task CanConnect();

    private async Task AssertConnectionStarted(bool anonymousLogin = false)
    {
        // Given
        var factory = new ConnectionFactory(_artemisContainer.GetBrokerAddress());

        // When
        if (!anonymousLogin)
        {
            var userInfo = new Uri(_artemisContainer.GetBrokerAddress()).UserInfo.Split(":");
            factory.UserName = userInfo[0];
            factory.Password = userInfo[1];
        }

        var connection = await factory.CreateConnectionAsync();
        await connection.StartAsync();

        // Then
        Assert.True(connection.IsStarted);
    }

    [UsedImplicitly]
    public sealed class DefaultCredentialsConfiguration : ArtemisContainerTest
    {
        public DefaultCredentialsConfiguration()
            : base(new ArtemisBuilder().Build())
        {
        }

        public override async Task CanConnect()
        {
            await AssertConnectionStarted();
        }
    }

    [UsedImplicitly]
    public sealed class CustomCredentialsConfiguration : ArtemisContainerTest
    {
        public CustomCredentialsConfiguration()
            : base(new ArtemisBuilder().WithUsername("testcontainers").WithPassword("testcontainers").Build())
        {
        }

        public override async Task CanConnect()
        {
            await AssertConnectionStarted();
        }
    }

    [UsedImplicitly]
    public sealed class AnonymousLoginConfiguration : ArtemisContainerTest
    {
        public AnonymousLoginConfiguration()
            : base(new ArtemisBuilder().WithEnvironment("ANONYMOUS_LOGIN", "true").Build())
        {
        }

        public override async Task CanConnect()
        {
            await AssertConnectionStarted(true);
        }
    }
}