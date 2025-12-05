namespace Testcontainers.EventHubs;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class EventHubsBuilder : ContainerBuilder<EventHubsBuilder, EventHubsContainer, EventHubsConfiguration>
{
    public const string EventHubsNetworkAlias = "eventhubs-container";

    public const string AzuriteNetworkAlias = "azurite-container";

    public const string EventHubsImage = "mcr.microsoft.com/azure-messaging/eventhubs-emulator:latest";

    public const ushort EventHubsPort = 5672;

    public const ushort EventHubsHttpPort = 5300;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public EventHubsBuilder()
        : this(EventHubsImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://mcr.microsoft.com/en-us/artifact/mar/azure-messaging/eventhubs-emulator/tags">https://mcr.microsoft.com/en-us/artifact/mar/azure-messaging/eventhubs-emulator/tags</see>.</param>
    public EventHubsBuilder(string image)
        : this(new EventHubsConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public EventHubsBuilder(IImage image)
        : this(new EventHubsConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private EventHubsBuilder(EventHubsConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override EventHubsConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    protected override string AcceptLicenseAgreementEnvVar { get; } = "ACCEPT_EULA";

    /// <inheritdoc />
    protected override string AcceptLicenseAgreement { get; } = "Y";

    /// <inheritdoc />
    protected override string DeclineLicenseAgreement { get; } = "N";

    /// <summary>
    /// Accepts the license agreement.
    /// </summary>
    /// <remarks>
    /// When <paramref name="acceptLicenseAgreement" /> is set to <c>true</c>, the Azure Event Hubs Emulator <see href="https://github.com/Azure/azure-event-hubs-emulator-installer/blob/main/EMULATOR_EULA.md">license</see> is accepted.
    /// </remarks>
    /// <param name="acceptLicenseAgreement">A boolean value indicating whether the Azure Event Hubs Emulator license agreement is accepted.</param>
    /// <returns>A configured instance of <see cref="EventHubsBuilder" />.</returns>
    public override EventHubsBuilder WithAcceptLicenseAgreement(bool acceptLicenseAgreement)
    {
        var licenseAgreement = acceptLicenseAgreement ? AcceptLicenseAgreement : DeclineLicenseAgreement;
        return WithEnvironment(AcceptLicenseAgreementEnvVar, licenseAgreement);
    }

    /// <summary>
    /// Sets the dependent Azurite container for the Azure Event Hubs Emulator.
    /// </summary>
    /// <remarks>
    /// This method allows an existing Azurite container to be attached to the Azure Event
    /// Hubs Emulator. The containers must be on the same network to enable communication
    /// between them.
    /// </remarks>
    /// <param name="network">The network to connect the container to.</param>
    /// <param name="container">The Azurite container.</param>
    /// <param name="networkAlias">The Azurite container network alias.</param>
    /// <returns>A configured instance of <see cref="EventHubsBuilder" />.</returns>
    public EventHubsBuilder WithAzuriteContainer(
        INetwork network,
        AzuriteContainer container,
        string networkAlias)
    {
        return Merge(DockerResourceConfiguration, new EventHubsConfiguration(azuriteContainer: container))
            .DependsOn(container)
            .WithNetwork(network)
            .WithNetworkAliases(EventHubsNetworkAlias)
            .WithEnvironment("BLOB_SERVER", networkAlias)
            .WithEnvironment("METADATA_SERVER", networkAlias);
    }

    /// <summary>
    /// Sets the Azure Event Hubs Emulator configuration.
    /// </summary>
    /// <param name="serviceConfiguration">The service configuration.</param>
    /// <returns>A configured instance of <see cref="EventHubsBuilder" />.</returns>
    public EventHubsBuilder WithConfigurationBuilder(EventHubsServiceConfiguration serviceConfiguration)
    {
        var resourceContent = Encoding.Default.GetBytes(serviceConfiguration.Build());
        return Merge(DockerResourceConfiguration, new EventHubsConfiguration(serviceConfiguration: serviceConfiguration))
            .WithResourceMapping(resourceContent, "Eventhubs_Emulator/ConfigFiles/Config.json");
    }

    /// <inheritdoc />
    public override EventHubsContainer Build()
    {
        Validate();
        ValidateLicenseAgreement();

        if (DockerResourceConfiguration.AzuriteContainer != null)
        {
            return new EventHubsContainer(DockerResourceConfiguration);
        }

        // If the user has not provided an existing Azurite container instance,
        // we configure one.
        var network = new NetworkBuilder()
            .Build();

        var container = new AzuriteBuilder()
            .WithNetwork(network)
            .WithNetworkAliases(AzuriteNetworkAlias)
            .Build();

        var eventHubsBuilder = WithAzuriteContainer(network, container, AzuriteNetworkAlias);
        return new EventHubsContainer(eventHubsBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.ServiceConfiguration, nameof(DockerResourceConfiguration.ServiceConfiguration))
            .NotNull()
            .ThrowIf(argument => !argument.Value.Validate(), _ => throw new ArgumentException("The service configuration of the Azure Event Hubs Emulator is invalid."));
    }

    /// <inheritdoc />
    protected override EventHubsBuilder Init()
    {
        return base.Init()
            .WithImage(EventHubsImage)
            .WithPortBinding(EventHubsPort, true)
            .WithPortBinding(EventHubsHttpPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPort(EventHubsHttpPort).ForPath("/health")));
    }

    /// <inheritdoc />
    protected override EventHubsBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new EventHubsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override EventHubsBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new EventHubsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override EventHubsBuilder Merge(EventHubsConfiguration oldValue, EventHubsConfiguration newValue)
    {
        return new EventHubsBuilder(new EventHubsConfiguration(oldValue, newValue));
    }
}