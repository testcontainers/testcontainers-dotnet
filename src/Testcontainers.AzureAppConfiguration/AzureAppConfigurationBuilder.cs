namespace Testcontainers.AzureAppConfiguration;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AzureAppConfigurationBuilder : ContainerBuilder<AzureAppConfigurationBuilder, AzureAppConfigurationContainer, AzureAppConfigurationConfiguration>
{
    public const string AzureAppConfigurationImage = "tnc1997/azure-app-configuration-emulator:1.0";

    public const ushort AzureAppConfigurationPort = 8080;

    public const string DefaultCredential = "abcd";

    public const string DefaultSecret = "c2VjcmV0";

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationBuilder" /> class.
    /// </summary>
    public AzureAppConfigurationBuilder()
        : this(new AzureAppConfigurationConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AzureAppConfigurationBuilder(AzureAppConfigurationConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override AzureAppConfigurationConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Azure App Configuration credential.
    /// </summary>
    /// <param name="credential">The Azure App Configuration credential.</param>
    /// <returns>A configured instance of <see cref="AzureAppConfigurationBuilder" />.</returns>
    public AzureAppConfigurationBuilder WithCredential(string credential)
    {
        return Merge(DockerResourceConfiguration, new AzureAppConfigurationConfiguration(credential: credential))
            .WithEnvironment("Authentication__Schemes__Hmac__Credential", credential);
    }

    /// <summary>
    /// Sets the Azure App Configuration secret.
    /// </summary>
    /// <param name="secret">The Azure App Configuration secret.</param>
    /// <returns>A configured instance of <see cref="AzureAppConfigurationBuilder" />.</returns>
    public AzureAppConfigurationBuilder WithSecret(string secret)
    {
        return Merge(DockerResourceConfiguration, new AzureAppConfigurationConfiguration(secret: secret))
            .WithEnvironment("Authentication__Schemes__Hmac__Secret", secret);
    }

    /// <inheritdoc />
    public override AzureAppConfigurationContainer Build()
    {
        Validate();
        return new AzureAppConfigurationContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override AzureAppConfigurationBuilder Init()
    {
        return base.Init()
            .WithImage(AzureAppConfigurationImage)
            .WithPortBinding(AzureAppConfigurationPort, true)
            .WithCredential(DefaultCredential)
            .WithSecret(DefaultSecret)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Now listening on"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Credential, nameof(DockerResourceConfiguration.Credential))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.Secret, nameof(DockerResourceConfiguration.Secret))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override AzureAppConfigurationBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzureAppConfigurationConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzureAppConfigurationBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzureAppConfigurationConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzureAppConfigurationBuilder Merge(AzureAppConfigurationConfiguration oldValue, AzureAppConfigurationConfiguration newValue)
    {
        return new AzureAppConfigurationBuilder(new AzureAppConfigurationConfiguration(oldValue, newValue));
    }
}