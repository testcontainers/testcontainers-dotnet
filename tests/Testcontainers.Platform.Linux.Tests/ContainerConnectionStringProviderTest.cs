namespace Testcontainers.Tests;

public sealed class ContainerConnectionStringProviderTests
{
    [Fact]
    public void ConfigureStoresContainerAndConfiguration()
    {
        var container = new Mock<IContainer>();
        var configuration = new Mock<IContainerConfiguration>();
        var provider = new InspectableProvider();

        provider.Configure(container.Object, configuration.Object);

        Assert.Same(container.Object, provider.ConfiguredContainer);
        Assert.Same(configuration.Object, provider.ConfiguredConfiguration);
    }

    [Fact]
    public void GetConnectionStringReturnsHostConnectionString()
    {
        var provider = new HostOnlyProvider("host-connection");

        var connectionString = provider.GetConnectionString();

        Assert.Equal("host-connection", connectionString);
    }

    [Fact]
    public void GetConnectionStringWithEmptyNameReturnsHostConnectionString()
    {
        var provider = new HostOnlyProvider("host-connection");

        var connectionString = provider.GetConnectionString(string.Empty);

        Assert.Equal("host-connection", connectionString);
    }

    [Fact]
    public void GetConnectionStringThrowsWhenHostConnectionStringIsMissing()
    {
        var provider = new HostOnlyProvider(string.Empty);

        Assert.Throws<ConnectionStringNotAvailableException>(() => provider.GetConnectionString());
    }

    [Fact]
    public void GetConnectionStringThrowsWhenContainerModeIsNotSupported()
    {
        var provider = new HostOnlyProvider("host-connection");

        Assert.Throws<ConnectionStringModeNotSupportedException>(() => provider.GetConnectionString(ConnectionMode.Container));
    }

    [Fact]
    public void GetConnectionStringReturnsContainerConnectionStringWhenSupported()
    {
        var provider = new HostAndContainerProvider("host-connection", "container-connection");

        var connectionString = provider.GetConnectionString(ConnectionMode.Container);

        Assert.Equal("container-connection", connectionString);
    }

    [Fact]
    public void GetConnectionStringThrowsWhenNamedConnectionStringIsRequested()
    {
        var provider = new HostOnlyProvider("host-connection");

        Assert.Throws<ConnectionStringNameNotSupportedException>(() => provider.GetConnectionString("primary"));
    }

    private sealed class InspectableProvider : ContainerConnectionStringProvider<IContainer, IContainerConfiguration>
    {
        public IContainer ConfiguredContainer => Container;

        public IContainerConfiguration ConfiguredConfiguration => Configuration;

        protected override string GetHostConnectionString()
        {
            return "host";
        }
    }

    private sealed class HostOnlyProvider : ContainerConnectionStringProvider<IContainer, IContainerConfiguration>
    {
        private readonly string _hostConnectionString;

        public HostOnlyProvider(string hostConnectionString)
        {
            _hostConnectionString = hostConnectionString;
        }

        protected override string GetHostConnectionString()
        {
            return _hostConnectionString;
        }
    }

    private sealed class HostAndContainerProvider : ContainerConnectionStringProvider<IContainer, IContainerConfiguration>
    {
        private readonly string _hostConnectionString;

        private readonly string _containerConnectionString;

        public HostAndContainerProvider(string hostConnectionString, string containerConnectionString)
        {
            _hostConnectionString = hostConnectionString;
            _containerConnectionString = containerConnectionString;
        }

        protected override string GetHostConnectionString()
        {
            return _hostConnectionString;
        }

        protected override string GetContainerConnectionString()
        {
            return _containerConnectionString;
        }
    }
}
