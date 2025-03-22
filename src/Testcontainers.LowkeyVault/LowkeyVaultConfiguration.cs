namespace Testcontainers.LowkeyVault;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class LowkeyVaultConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultConfiguration" /> class.
    /// </summary>
    /// <param name="additionalArguments">The additional arguments to be passed via environment variables to the container.</param>
    public LowkeyVaultConfiguration(List<string> additionalArguments = null)
    {
        AdditionalArguments = additionalArguments;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LowkeyVaultConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LowkeyVaultConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LowkeyVaultConfiguration(LowkeyVaultConfiguration resourceConfiguration)
        : this(new LowkeyVaultConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public LowkeyVaultConfiguration(LowkeyVaultConfiguration oldValue, LowkeyVaultConfiguration newValue)
        : base(oldValue, newValue)
    {
        AdditionalArguments = BuildConfiguration.Combine(oldValue.AdditionalArguments, newValue.AdditionalArguments);
    }

    /// <summary>
    /// Gets Additional Arguments.
    /// </summary>
    internal List<string> AdditionalArguments { get; } = [];
}