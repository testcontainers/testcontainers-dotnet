namespace Testcontainers.Tests;

public sealed class ConnectionStringProviderTests : IAsyncLifetime
{
    private const string ExpectedConnectionString = "connection string";

    private readonly IContainer _container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithCommand(CommonCommands.SleepInfinity)
        .WithConnectionStringProvider(new ConnectionStringProvider())
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public void GetConnectionStringReturnsExpectedValue()
    {
        Assert.Equal(ExpectedConnectionString, _container.GetConnectionString());
        Assert.Equal(ExpectedConnectionString, _container.GetConnectionString("name"));
    }

    private sealed class ConnectionStringProvider : IConnectionStringProvider<IContainer, IContainerConfiguration>
    {
        public void Configure(IContainer container, IContainerConfiguration configuration)
        {
            Assert.NotNull(container);
            Assert.NotNull(configuration);
        }

        public string GetConnectionString(ConnectionMode connectionMode = ConnectionMode.Host)
        {
            return ExpectedConnectionString;
        }

        public string GetConnectionString(string name, ConnectionMode connectionMode = ConnectionMode.Host)
        {
            return ExpectedConnectionString;
        }
    }
}