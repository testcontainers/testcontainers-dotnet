namespace Testcontainers.Kusto;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
/// <remarks>
/// Builds a container running the Azure Data Explorer Kusto emulator:
/// https://learn.microsoft.com/azure/data-explorer/kusto-emulator-overview.
/// </remarks>
[PublicAPI]
public sealed class KustoBuilder : ContainerBuilder<KustoBuilder, KustoContainer, KustoConfiguration>
{
    public const string KustoImage = "mcr.microsoft.com/azuredataexplorer/kustainer-linux:latest";

    public const ushort KustoPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoBuilder" /> class.
    /// </summary>
    public KustoBuilder()
        : this(new KustoConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private KustoBuilder(KustoConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override KustoConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override KustoContainer Build()
    {
        Validate();
        return new KustoContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override KustoBuilder Init()
    {
        return base.Init()
            .WithImage(KustoImage)
            .WithPortBinding(KustoPort, true)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request
                .WithMethod(HttpMethod.Post)
                .ForPort(KustoPort)
                .ForPath("/v1/rest/mgmt")
                .WithContent(() => new StringContent("{\"csl\":\".show cluster\"}", Encoding.Default, "application/json"))));
    }

    /// <inheritdoc />
    protected override KustoBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KustoConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KustoBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KustoConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KustoBuilder Merge(KustoConfiguration oldValue, KustoConfiguration newValue)
    {
        return new KustoBuilder(new KustoConfiguration(oldValue, newValue));
    }
}