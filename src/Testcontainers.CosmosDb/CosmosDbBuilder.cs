using System.Net;
using System.Net.Sockets;

namespace Testcontainers.CosmosDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CosmosDbBuilder : ContainerBuilder<CosmosDbBuilder, CosmosDbContainer, CosmosDbConfiguration>
{
    public const string CosmosDbImage = "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview";
    public const string DefaultAccountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    public readonly ushort CosmosDbPort;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbBuilder" /> class.
    /// </summary>
    public CosmosDbBuilder()
        : this(new CosmosDbConfiguration())
    {
        CosmosDbPort = GetAvailablePort();
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CosmosDbBuilder(CosmosDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override CosmosDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override CosmosDbContainer Build()
    {
        Validate();
        return new CosmosDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Init()
    {
        return base.Init()
            .WithImage(CosmosDbImage)
            .WithEnvironment("ENABLE_EXPLORER", "false")
            .WithEnvironment("PORT", CosmosDbPort.ToString())
            .WithPortBinding(CosmosDbPort, CosmosDbPort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPort(CosmosDbPort)));
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CosmosDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CosmosDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CosmosDbBuilder Merge(CosmosDbConfiguration oldValue, CosmosDbConfiguration newValue)
    {
        return new CosmosDbBuilder(new CosmosDbConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Gets an available port.
    /// </summary>
    private static ushort GetAvailablePort()
    {
#if NET8_0_OR_GREATER
        using (var listener = new TcpListener(IPAddress.Loopback, 0))
        {
            listener.Start();
            return (ushort)((IPEndPoint)listener.LocalEndpoint).Port;
        }
#else
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var port = (ushort)((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
#endif
    }
}