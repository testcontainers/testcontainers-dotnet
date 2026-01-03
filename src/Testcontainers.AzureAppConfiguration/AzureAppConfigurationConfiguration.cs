namespace Testcontainers.AzureAppConfiguration;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class AzureAppConfigurationConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationConfiguration" /> class.
    /// </summary>
    /// <param name="credential">The Azure App Configuration credential.</param>
    /// <param name="secret">The Azure App Configuration secret.</param>
    public AzureAppConfigurationConfiguration(string credential = null, string secret = null)
    {
        Credential = credential;
        Secret = secret;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzureAppConfigurationConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzureAppConfigurationConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzureAppConfigurationConfiguration(AzureAppConfigurationConfiguration resourceConfiguration)
        : this(new AzureAppConfigurationConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public AzureAppConfigurationConfiguration(AzureAppConfigurationConfiguration oldValue, AzureAppConfigurationConfiguration newValue)
        : base(oldValue, newValue)
    {
        Credential = BuildConfiguration.Combine(oldValue.Credential, newValue.Credential);
        Secret = BuildConfiguration.Combine(oldValue.Secret, newValue.Secret);
    }

    /// <summary>
    /// Gets the Azure App Configuration credential.
    /// </summary>
    public string Credential { get; }

    /// <summary>
    /// Gets the Azure App Configuration secret.
    /// </summary>
    public string Secret { get; }
}