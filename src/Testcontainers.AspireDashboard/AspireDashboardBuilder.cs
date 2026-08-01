namespace Testcontainers.AspireDashboard;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AspireDashboardBuilder : ContainerBuilder<AspireDashboardBuilder, AspireDashboardContainer, AspireDashboardConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string AspireDashboardImage = "mcr.microsoft.com/dotnet/aspire-dashboard:13";

    public const ushort AspireDashboardHttpPort = 18888;

    public const ushort AspireDashboardOtlpGrpcPort = 18889;

    public const ushort AspireDashboardOtlpHttpPort = 18890;

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public AspireDashboardBuilder()
        : this(AspireDashboardImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>mcr.microsoft.com/dotnet/aspire-dashboard:13</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/microsoft/dotnet-aspire-dashboard" />.
    /// </remarks>
    public AspireDashboardBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/microsoft/dotnet-aspire-dashboard" />.
    /// </remarks>
    public AspireDashboardBuilder(IImage image)
        : this(new AspireDashboardConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AspireDashboardBuilder(AspireDashboardConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override AspireDashboardConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override AspireDashboardContainer Build()
    {
        Validate();
        return new AspireDashboardContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override AspireDashboardBuilder Init()
    {
        return base.Init()
            .WithImage(AspireDashboardImage)
            .WithPortBinding(AspireDashboardHttpPort, true)
            .WithPortBinding(AspireDashboardOtlpGrpcPort, true)
            .WithPortBinding(AspireDashboardOtlpHttpPort, true)
            .WithEnvironment("DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS", "true")
            .WithConnectionStringProvider(new AspireDashboardConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPort(AspireDashboardHttpPort)));
    }

    /// <inheritdoc />
    protected override AspireDashboardBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AspireDashboardConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AspireDashboardBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AspireDashboardConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AspireDashboardBuilder Merge(AspireDashboardConfiguration oldValue, AspireDashboardConfiguration newValue)
    {
        return new AspireDashboardBuilder(new AspireDashboardConfiguration(oldValue, newValue));
    }
}