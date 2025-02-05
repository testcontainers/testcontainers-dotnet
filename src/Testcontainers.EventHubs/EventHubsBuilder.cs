namespace Testcontainers.EventHubs;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class EventHubsBuilder : ContainerBuilder<EventHubsBuilder, EventHubsContainer, EventHubsConfiguration>
{
    public const string EventHubsNetworkAlias = "eventhubs-container";

    public const string AzuriteNetworkAlias = "azurite-container";

    public const string EventHubsImage = "mcr.microsoft.com/azure-messaging/eventhubs-emulator:latest";

    public const ushort EventHubsPort = 5672;

    public const ushort KafkaPort = 9092;

    private const string AcceptLicenseAgreementEnvVar = "ACCEPT_EULA";

    private const string AcceptLicenseAgreement = "Y";

    private const string DeclineLicenseAgreement = "N";

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsBuilder" /> class.
    /// </summary>
    public EventHubsBuilder()
        : this(new EventHubsConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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

    /// <summary>
    /// Accepts the license agreement.
    /// </summary>
    /// <remarks>
    /// When <paramref name="acceptLicenseAgreement" /> is set to <c>true</c>, the Azure Event Hubs Emulator <see href="https://github.com/Azure/azure-event-hubs-emulator-installer/blob/main/EMULATOR_EULA.md">license</see> is accepted.
    /// </remarks>
    /// <param name="acceptLicenseAgreement">A boolean value indicating whether the Azure Event Hubs Emulator license agreement is accepted.</param>
    /// <returns>A configured instance of <see cref="EventHubsBuilder" />.</returns>
    public EventHubsBuilder WithAcceptLicenseAgreement(bool acceptLicenseAgreement)
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
    /// <param name="container">The Azure container.</param>
    /// <param name="networkAlias">The Azure container network alias.</param>
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
    /// <param name="configurationBuilder">The configuration.</param>
    /// <returns>A configured instance of <see cref="EventHubsBuilder" />.</returns>
    public EventHubsBuilder WithConfigurationBuilder(ConfigurationBuilder configurationBuilder)
    {
        var resourceContent = Encoding.Default.GetBytes(configurationBuilder.Build());
        return Merge(DockerResourceConfiguration, new EventHubsConfiguration(configurationBuilder: configurationBuilder))
            .WithResourceMapping(resourceContent, "Eventhubs_Emulator/ConfigFiles/Config.json");
    }

    /// <inheritdoc />
    public override EventHubsContainer Build()
    {
        Validate();

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

        var eventHubsContainer = WithAzuriteContainer(network, container, AzuriteNetworkAlias);
        return new EventHubsContainer(eventHubsContainer.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string message = "The image '{0}' requires you to accept a license agreement.";

        base.Validate();

        Predicate<EventHubsConfiguration> licenseAgreementNotAccepted = value =>
            !value.Environments.TryGetValue(AcceptLicenseAgreementEnvVar, out var licenseAgreementValue) || !AcceptLicenseAgreement.Equals(licenseAgreementValue, StringComparison.Ordinal);

        _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration.Image))
            .ThrowIf(argument => licenseAgreementNotAccepted(argument.Value), argument => throw new ArgumentException(string.Format(message, DockerResourceConfiguration.Image.FullName), argument.Name));

        _ = Guard.Argument(DockerResourceConfiguration.ConfigurationBuilder, nameof(DockerResourceConfiguration.ConfigurationBuilder))
            .NotNull()
            .ThrowIf(argument => !argument.Value.Validate(), _ => throw new ArgumentException("ConfigurationBuilder is invalid."));
    }

    /// <inheritdoc />
    protected override EventHubsBuilder Init()
    {
        return base.Init()
            .WithImage(EventHubsImage)
            .WithNetworkAliases(EventHubsNetworkAlias)
            .WithPortBinding(EventHubsPort, true)
            .WithPortBinding(KafkaPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("Emulator Service is Successfully Up!")
                .AddCustomWaitStrategy(new WaitTwoSeconds()));
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

    /// <inheritdoc />
    private sealed class WaitTwoSeconds : IWaitUntil
    {
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            await Task.Delay(TimeSpan.FromSeconds(2))
                .ConfigureAwait(false);

            return true;
        }
    }
}