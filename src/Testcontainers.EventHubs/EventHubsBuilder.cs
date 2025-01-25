namespace Testcontainers.EventHubs;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class EventHubsBuilder : ContainerBuilder<EventHubsBuilder, EventHubsContainer, EventHubsConfiguration>
{
    public const string EventHubsImage = "mcr.microsoft.com/azure-messaging/eventhubs-emulator:latest";

    public const ushort EventHubsPort = 5672;

    public const string EventHubNetworkAlias = "eventhub";
    public const string AzuriteNetworkAlias = "azurite";

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
    /// Sets the event hub configuration
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <returns></returns>
    public EventHubsBuilder WithConfigurationBuilder(ConfigurationBuilder configurationBuilder)
    {
        var configBytes = Encoding.UTF8.GetBytes(configurationBuilder.Build());

        return Merge(DockerResourceConfiguration,
                new EventHubsConfiguration(configurationBuilder: configurationBuilder))
            .WithResourceMapping(configBytes, "Eventhubs_Emulator/ConfigFiles/Config.json");
    }

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
    ///  Sets the Azurite container for the Event Hubs Emulator.
    /// </summary>
    /// <param name="azuriteContainer">docker container</param>
    /// <param name="alias">network alias name that is used for connection to blob and table endpoints</param>
    /// <returns></returns>
    public EventHubsBuilder WithAzurite(AzuriteContainer azuriteContainer,
        string alias) =>
        WithAzurite(azuriteContainer, false, alias);

    private EventHubsBuilder WithAzurite(AzuriteContainer azuriteContainer,
        bool isInternal,
        string alias = AzuriteNetworkAlias)
    {
        var builder = Merge(DockerResourceConfiguration,
            new EventHubsConfiguration(azuriteContainer: azuriteContainer));

        if (!isInternal)
        {
            builder = builder.DependsOn(azuriteContainer);
        }

        return builder
            .WithEnvironment("BLOB_SERVER", alias)
            .WithEnvironment("METADATA_SERVER", alias);
    }

    private EventHubsBuilder WithAzurite()
    {
        var azuriteContainer = new AzuriteBuilder()
            .WithNetwork(DockerResourceConfiguration.Networks.Single())
            .WithNetworkAliases(AzuriteNetworkAlias)
            .Build();

        return WithAzurite(azuriteContainer, true);
    }

    /// <inheritdoc />
    public override EventHubsContainer Build()
    {
        Validate();

        var builder = this;
        
        if (!DockerResourceConfiguration.Networks.Any())
        {
            builder = builder.WithNetwork(new NetworkBuilder().Build());
        }
        
        if (DockerResourceConfiguration.AzuriteContainer == null)
        {
            builder = builder.WithAzurite();
        }
        
        return new EventHubsContainer(builder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration.Image))
            .ThrowIf(argument => LicenseAgreementNotAccepted(argument.Value),
                argument => throw new ArgumentException(
                    $"The image '{DockerResourceConfiguration.Image.FullName}' requires you to accept a license agreement.",
                    argument.Name));

        _ = Guard.Argument(DockerResourceConfiguration.ConfigurationBuilder,
                nameof(DockerResourceConfiguration.ConfigurationBuilder))
            .NotNull()
            .ThrowIf(x => !x.Value.Validate(), _ => throw new ArgumentException("ConfigurationBuilder is invalid."));

        return;

        bool LicenseAgreementNotAccepted(EventHubsConfiguration value) =>
            !value.Environments.TryGetValue(AcceptLicenseAgreementEnvVar, out var licenseAgreementValue) ||
            !AcceptLicenseAgreement.Equals(licenseAgreementValue, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    protected override EventHubsBuilder Init()
    {
        return base.Init()
            .WithImage(EventHubsImage)
            .WithNetworkAliases(EventHubNetworkAlias)
            .WithPortBinding(EventHubsPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("Emulator Service is Successfully Up!")
                .AddCustomWaitStrategy(new WaitTwoSeconds()));
        ;
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
    protected override EventHubsBuilder Merge(EventHubsConfiguration oldValue,
        EventHubsConfiguration newValue)
    {
        return new EventHubsBuilder(new EventHubsConfiguration(oldValue, newValue));
    }

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