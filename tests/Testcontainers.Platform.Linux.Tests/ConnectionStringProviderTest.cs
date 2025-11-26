namespace Testcontainers.Tests;

public sealed class ConnectionStringProviderTests : IAsyncLifetime
{
    private const string ExpectedConnectionString = "connection string";

    private readonly ConnectionStringProvider _connectionStringProvider = new ConnectionStringProvider();

    private readonly IContainer _container;

    public ConnectionStringProviderTests()
    {
        _container = new ContainerBuilder()
            .WithImage(CommonImages.Alpine)
            .WithCommand(CommonCommands.SleepInfinity)
            .WithConnectionStringProvider(_connectionStringProvider)
            .Build();
    }

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
        Assert.True(_connectionStringProvider.IsConfigured, "Configure should have been called during container startup.");
        Assert.Equal(ExpectedConnectionString, _container.GetConnectionString());
        Assert.Equal(ExpectedConnectionString, _container.GetConnectionString("name"));
    }

    private sealed class ConnectionStringProvider : IConnectionStringProvider<IContainer, IContainerConfiguration>
    {
        public bool IsConfigured { get; private set; }

        public void Configure(IContainer container, IContainerConfiguration configuration)
        {
            Assert.NotNull(container);
            Assert.NotNull(configuration);
            IsConfigured = true;
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