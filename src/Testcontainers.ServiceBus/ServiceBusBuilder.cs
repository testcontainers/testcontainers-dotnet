namespace Testcontainers.ServiceBus;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ServiceBusBuilder : ContainerBuilder<ServiceBusBuilder, ServiceBusContainer, ServiceBusConfiguration>
{
    public const string ServiceBusNetworkAlias = "servicebus-container";

    public const string DatabaseNetworkAlias = "database-container";

    public const string ServiceBusImage = "mcr.microsoft.com/azure-messaging/servicebus-emulator:latest";

    public const ushort ServiceBusPort = 5672;

    private const string AcceptLicenseAgreementEnvVar = "ACCEPT_EULA";

    private const string AcceptLicenseAgreement = "Y";

    private const string DeclineLicenseAgreement = "N";

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

    /// <summary>
    /// Accepts the license agreement.
    /// </summary>
    /// <remarks>
    /// When <paramref name="acceptLicenseAgreement" /> is set to <c>true</c>, the Azure Service Bus Emulator <see href="https://github.com/Azure/azure-service-bus-emulator-installer/blob/main/EMULATOR_EULA.txt">license</see> is accepted.
    /// </remarks>
    /// <param name="acceptLicenseAgreement">A boolean value indicating whether the Azure Service Bus Emulator license agreement is accepted.</param>
    /// <returns>A configured instance of <see cref="ServiceBusBuilder" />.</returns>
    public ServiceBusBuilder WithAcceptLicenseAgreement(bool acceptLicenseAgreement)
    {
        var licenseAgreement = acceptLicenseAgreement ? AcceptLicenseAgreement : DeclineLicenseAgreement;
        return WithEnvironment(AcceptLicenseAgreementEnvVar, licenseAgreement);
    }

    /// <inheritdoc />
    public override ServiceBusContainer Build()
    {
        Validate();
        return new ServiceBusContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string message = "The image '{0}' requires you to accept a license agreement.";

        base.Validate();

        Predicate<ServiceBusConfiguration> licenseAgreementNotAccepted = value =>
            !value.Environments.TryGetValue(AcceptLicenseAgreementEnvVar, out var licenseAgreementValue) || !AcceptLicenseAgreement.Equals(licenseAgreementValue, StringComparison.Ordinal);

        _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration.Image))
            .ThrowIf(argument => licenseAgreementNotAccepted(argument.Value), argument => throw new ArgumentException(string.Format(message, DockerResourceConfiguration.Image.FullName), argument.Name));
    }

    /// <inheritdoc />
    protected override ServiceBusBuilder Init()
    {
        return base.Init()
            .WithImage(ServiceBusImage)
            .WithNetwork(new NetworkBuilder().Build())
            .WithNetworkAliases(ServiceBusNetworkAlias)
            .WithPortBinding(ServiceBusPort, true)
            .WithMsSqlContainer()
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

    /// <summary>
    /// Configures the dependent MSSQL container.
    /// </summary>
    /// <returns>A configured instance of <see cref="ServiceBusBuilder" />.</returns>
    private ServiceBusBuilder WithMsSqlContainer()
    {
        var msSqlContainer = new MsSqlBuilder()
            .WithNetwork(DockerResourceConfiguration.Networks.Single())
            .WithNetworkAliases(DatabaseNetworkAlias)
            .Build();

        return Merge(DockerResourceConfiguration, new ServiceBusConfiguration(databaseContainer: msSqlContainer))
            .WithEnvironment("MSSQL_SA_PASSWORD", MsSqlBuilder.DefaultPassword)
            .WithEnvironment("SQL_SERVER", DatabaseNetworkAlias);
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