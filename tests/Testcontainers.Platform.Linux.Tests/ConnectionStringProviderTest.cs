namespace Testcontainers.Tests;

public static class ConnectionStringProviderTests
{
    private const string ExpectedConnectionString = "connection string";

    public sealed class Configured : IAsyncLifetime
    {
        private readonly ConnectionStringProvider _connectionStringProvider = new ConnectionStringProvider();

        private readonly IContainer _container;

        public Configured()
        {
            _container = new ContainerBuilder(CommonImages.Alpine)
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
            IConnectionStringProvider connectionStringProvider = _container;
            Assert.True(_connectionStringProvider.IsConfigured, "Configure should have been called during container startup.");
            Assert.Equal(ExpectedConnectionString, connectionStringProvider.GetConnectionString());
            Assert.Equal(ExpectedConnectionString, connectionStringProvider.GetConnectionString("name"));
        }
    }

    public sealed class NotConfigured : IAsyncLifetime
    {
        private readonly IContainer _container = new ContainerBuilder(CommonImages.Alpine)
            .WithCommand(CommonCommands.SleepInfinity)
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
        public void GetConnectionStringThrowsException()
        {
            IConnectionStringProvider connectionStringProvider = _container;
            Assert.Throws<ConnectionStringProviderNotConfiguredException>(() => connectionStringProvider.GetConnectionString());
            Assert.Throws<ConnectionStringProviderNotConfiguredException>(() => connectionStringProvider.GetConnectionString("name"));
        }
    }

    public sealed class Shared : IAsyncLifetime
    {
        private readonly IContainer _container1;

        private readonly IContainer _container2;

        public Shared()
        {
            var containerBuilder = new ContainerBuilder(CommonImages.Alpine)
                .WithCommand(CommonCommands.SleepInfinity)
                .WithConnectionStringProvider(new ContainerIdConnectionStringProvider());

            _container1 = containerBuilder.Build();
            _container2 = containerBuilder.Build();
        }

        public async ValueTask InitializeAsync()
        {
            await _container1.StartAsync()
                .ConfigureAwait(false);

            await _container2.StartAsync()
                .ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await _container1.DisposeAsync()
                .ConfigureAwait(false);

            await _container2.DisposeAsync()
                .ConfigureAwait(false);
        }

        [Fact]
        public void GetConnectionStringReturnsValueOfItsOwnContainer()
        {
            Assert.Equal(_container1.Id, _container1.GetConnectionString());
            Assert.Equal(_container2.Id, _container2.GetConnectionString());
        }
    }

    private sealed class ContainerIdConnectionStringProvider : ContainerConnectionStringProvider<IContainer, IContainerConfiguration>
    {
        protected override string GetHostConnectionString()
        {
            return Container.Id;
        }
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