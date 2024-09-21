namespace Testcontainers.AspireDashboard;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AspireDashboardBuilder
    : ContainerBuilder<
        AspireDashboardBuilder,
        AspireDashboardContainer,
        AspireDashboardConfiguration
    >
{
    // https://mcr.microsoft.com/en-us/product/dotnet/aspire-dashboard/tags
    public const string AspireDashboardImage = "mcr.microsoft.com/dotnet/aspire-dashboard:8.1.0";

    public const ushort AspireDashboardFrontendPort = 18888;

    public const ushort AspireDashboardOtlpPort = 18889;

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardBuilder" /> class.
    /// </summary>
    public AspireDashboardBuilder()
        : this(new AspireDashboardConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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

    /// <summary>
    /// Configures the dashboard to accept anonymous access.
    /// </summary>
    /// <param name="allowed">A value indicating whether anonymous access is allowed.</param>
    /// <returns>A configured instance of <see cref="AspireDashboardBuilder" />.</returns>
    public AspireDashboardBuilder AllowAnonymous(bool allowed)
    {
        return Merge(DockerResourceConfiguration, new AspireDashboardConfiguration())
            .WithEnvironment(
                "DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS",
                allowed.ToString().ToLowerInvariant()
            );
    }

    /// <summary>
    /// Configures the dashboard to allow unsecured transport.
    /// </summary>
    /// <param name="allowed">A value indicating whether unsecured transport is allowed.</param>
    /// <returns>A configured instance of <see cref="AspireDashboardBuilder" />.</returns>
    public AspireDashboardBuilder AllowUnsecuredTransport(bool allowed)
    {
        return Merge(DockerResourceConfiguration, new AspireDashboardConfiguration())
            .WithEnvironment(
                "ASPIRE_ALLOW_UNSECURED_TRANSPORT",
                allowed.ToString().ToLowerInvariant()
            );
    }

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
            .WithPortBinding(AspireDashboardFrontendPort, true)
            .WithPortBinding(AspireDashboardOtlpPort, true)
            .AllowAnonymous(true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r => r.ForPort(AspireDashboardFrontendPort))
            );
    }

    /// <inheritdoc />
    protected override AspireDashboardBuilder Clone(
        IResourceConfiguration<CreateContainerParameters> resourceConfiguration
    )
    {
        return Merge(
            DockerResourceConfiguration,
            new AspireDashboardConfiguration(resourceConfiguration)
        );
    }

    /// <inheritdoc />
    protected override AspireDashboardBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(
            DockerResourceConfiguration,
            new AspireDashboardConfiguration(resourceConfiguration)
        );
    }

    /// <inheritdoc />
    protected override AspireDashboardBuilder Merge(
        AspireDashboardConfiguration oldValue,
        AspireDashboardConfiguration newValue
    )
    {
        return new AspireDashboardBuilder(new AspireDashboardConfiguration(oldValue, newValue));
    }
}
