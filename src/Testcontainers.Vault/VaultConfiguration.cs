namespace Testcontainers.Vault;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class VaultConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VaultConfiguration" /> class.
    /// </summary>
    /// <param name="rootToken">The Vault root token.</param>
    /// <param name="initCommands">The Vault CLI commands that run after the container has started.</param>
    public VaultConfiguration(
        string rootToken = null,
        IReadOnlyList<string> initCommands = null)
    {
        RootToken = rootToken;
        InitCommands = initCommands;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public VaultConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public VaultConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public VaultConfiguration(VaultConfiguration resourceConfiguration)
        : this(new VaultConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public VaultConfiguration(VaultConfiguration oldValue, VaultConfiguration newValue)
        : base(oldValue, newValue)
    {
        RootToken = BuildConfiguration.Combine(oldValue.RootToken, newValue.RootToken);
        InitCommands = BuildConfiguration.Combine(oldValue.InitCommands, newValue.InitCommands);
    }

    /// <summary>
    /// Gets the Vault root token.
    /// </summary>
    public string RootToken { get; }

    /// <summary>
    /// Gets the Vault CLI commands that run after the container has started.
    /// </summary>
    public IReadOnlyList<string> InitCommands { get; }
}
