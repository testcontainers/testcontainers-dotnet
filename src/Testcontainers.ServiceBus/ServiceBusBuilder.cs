namespace Testcontainers.ServiceBus;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ServiceBusBuilder : ContainerBuilder<ServiceBusBuilder, ServiceBusContainer, ServiceBusConfiguration>
{
    public const string ServiceBusNetworkAlias = "servicebus-container";

    public const string DatabaseNetworkAlias = "database-container";

    public const string ServiceBusImage = "mcr.microsoft.com/azure-messaging/servicebus-emulator:latest";

    public const ushort ServiceBusPort = 5672;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusBuilder" /> class.
    /// </summary>
    public ServiceBusBuilder()
        : this(new ServiceBusConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ServiceBusBuilder(ServiceBusConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ServiceBusConfiguration DockerResourceConfiguration { get; }

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
    /// When <paramref name="acceptLicenseAgreement" /> is set to <c>true</c>, the Azure Service Bus Emulator <see href="https://github.com/Azure/azure-service-bus-emulator-installer/blob/main/EMULATOR_EULA.txt">license</see> is accepted.
    /// </remarks>
    /// <param name="acceptLicenseAgreement">A boolean value indicating whether the Azure Service Bus Emulator license agreement is accepted.</param>
    /// <returns>A configured instance of <see cref="ServiceBusBuilder" />.</returns>
    public override ServiceBusBuilder WithAcceptLicenseAgreement(bool acceptLicenseAgreement)
    {
        var licenseAgreement = acceptLicenseAgreement ? AcceptLicenseAgreement : DeclineLicenseAgreement;
        return WithEnvironment(AcceptLicenseAgreementEnvVar, licenseAgreement);
    }

    /// <summary>
    /// Sets the dependent MSSQL container for the Azure Service Bus Emulator.
    /// </summary>
    /// <remarks>
    /// This method allows an existing MSSQL container to be attached to the Azure Service
    /// Bus Emulator. The containers must be on the same network to enable communication
    /// between them.
    /// </remarks>
    /// <param name="network">The network to connect the container to.</param>
    /// <param name="container">The MSSQL container.</param>
    /// <param name="networkAlias">The MSSQL container network alias.</param>
    /// <param name="password">The MSSQL container password.</param>
    /// <returns>A configured instance of <see cref="ServiceBusBuilder" />.</returns>
    public ServiceBusBuilder WithMsSqlContainer(
        INetwork network,
        MsSqlContainer container,
        string networkAlias,
        string password = MsSqlBuilder.DefaultPassword)
    {
        return Merge(DockerResourceConfiguration, new ServiceBusConfiguration(databaseContainer: container))
            .DependsOn(container)
            .WithNetwork(network)
            .WithNetworkAliases(ServiceBusNetworkAlias)
            .WithEnvironment("SQL_SERVER", networkAlias)
            .WithEnvironment("MSSQL_SA_PASSWORD", password);
    }

    /// <inheritdoc />
    public override ServiceBusContainer Build()
    {
        Validate();
        ValidateLicenseAgreement();

        if (DockerResourceConfiguration.DatabaseContainer != null)
        {
            return new ServiceBusContainer(DockerResourceConfiguration);
        }

        // If the user has not provided an existing MSSQL container instance,
        // we configure one.
        var network = new NetworkBuilder()
            .Build();

        var container = new MsSqlBuilder()
            .WithNetwork(network)
            .WithNetworkAliases(DatabaseNetworkAlias)
            .Build();

        var serviceBusBuilder = WithMsSqlContainer(network, container, DatabaseNetworkAlias);
        return new ServiceBusContainer(serviceBusBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ServiceBusBuilder Init()
    {
        return base.Init()
            .WithImage(ServiceBusImage)
            .WithPortBinding(ServiceBusPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("Emulator Service is Successfully Up!")
                .AddCustomWaitStrategy(new WaitTwoSeconds()));
    }

    /// <inheritdoc />
    protected override ServiceBusBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ServiceBusConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ServiceBusBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ServiceBusConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ServiceBusBuilder Merge(ServiceBusConfiguration oldValue, ServiceBusConfiguration newValue)
    {
        return new ServiceBusBuilder(new ServiceBusConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    /// <remarks>
    /// This is a workaround to ensure that the wait strategy does not indicate
    /// readiness too early:
    /// https://github.com/Azure/azure-service-bus-emulator-installer/issues/35#issuecomment-2497164533.
    /// </remarks>
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