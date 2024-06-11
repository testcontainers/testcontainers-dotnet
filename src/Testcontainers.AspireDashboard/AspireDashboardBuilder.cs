namespace Testcontainers.AspireDashboard;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AspireDashboardBuilder : ContainerBuilder<AspireDashboardBuilder, AspireDashboardContainer, AspireDashboardConfiguration>
{
    public const string AspireDashboardImage = "mcr.microsoft.com/dotnet/aspire-dashboard:latest";

    public const ushort AspireDashboardPort = 18888;

    public const ushort AspireDashboardOtlpPort = 18889;

    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardBuilder" /> class.
    /// </summary>
    public AspireDashboardBuilder()
        : this(new AspireDashboardConfiguration(false, false))
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

    /// <summary>
    /// Sets the AllowAnonymous mode.
    /// </summary>
    /// <param name="allowed"></param>
    /// <returns>A configured instance of <see cref="KeycloakBuilder" />.</returns>
    public AspireDashboardBuilder AllowAnonymous(bool allowed)
    {
        return Merge(DockerResourceConfiguration, new AspireDashboardConfiguration(allowAnonymous: allowed))
            .WithEnvironment("DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS", allowed.ToString().ToLower());
    }

    /// <summary>
    /// Sets the AllowAnonymous mode.
    /// </summary>
    /// <param name="allowed"></param>
    /// <returns>A configured instance of <see cref="KeycloakBuilder" />.</returns>
    public AspireDashboardBuilder AllowUnsecuredTransport(bool allowed)
    {
        return Merge(DockerResourceConfiguration, new AspireDashboardConfiguration(allowUnsecuredTransport: allowed))
            .WithEnvironment("ASPIRE_ALLOW_UNSECURED_TRANSPORT", allowed.ToString().ToLower());
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
            .WithPortBinding(AspireDashboardPort, AspireDashboardPort)
            .WithPortBinding(AspireDashboardOtlpPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(AspireDashboardPort)));
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