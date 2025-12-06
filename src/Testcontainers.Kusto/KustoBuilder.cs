namespace Testcontainers.Kusto;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
/// <remarks>
/// Builds a container running the Azure Data Explorer Kusto emulator:
/// https://learn.microsoft.com/azure/data-explorer/kusto-emulator-overview.
/// </remarks>
[PublicAPI]
public sealed class KustoBuilder : ContainerBuilder<KustoBuilder, KustoContainer, KustoConfiguration>
{
    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string KustoImage = "mcr.microsoft.com/azuredataexplorer/kustainer-linux:latest";

    public const ushort KustoPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public KustoBuilder()
        : this(KustoImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag.</param>
    public KustoBuilder(string image)
        : this(new KustoConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public KustoBuilder(IImage image)
        : this(new KustoConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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
        return new KustoContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override KustoBuilder Init()
    {
        return base.Init()
            .WithPortBinding(KustoPort, true)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request
                .WithMethod(HttpMethod.Post)
                .ForPort(KustoPort)
                .ForPath("/v1/rest/mgmt")
                .WithContent(() => new StringContent("{\"csl\":\".show cluster\"}", Encoding.UTF8, "application/json"))));
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