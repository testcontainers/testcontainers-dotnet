namespace Testcontainers.LowkeyVault;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class LowkeyVaultConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultConfiguration" /> class.
    /// </summary>
    /// <param name="arguments">The arguments to add to the <c>LOWKEY_ARGS</c> environment variable.</param>
    public LowkeyVaultConfiguration(IEnumerable<string> arguments = null)
    {
        Arguments = arguments;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LowkeyVaultConfiguration(
        IResourceConfiguration<CreateContainerParameters> resourceConfiguration
    )
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
    public LowkeyVaultConfiguration(
        LowkeyVaultConfiguration oldValue,
        LowkeyVaultConfiguration newValue
    )
        : base(oldValue, newValue)
    {
        Arguments = BuildConfiguration.Combine(oldValue.Arguments, newValue.Arguments);
    }

    /// <summary>
    /// Gets the arguments that are added to the <c>LOWKEY_ARGS</c> environment variable.
    /// </summary>
    public IEnumerable<string> Arguments { get; }
}
