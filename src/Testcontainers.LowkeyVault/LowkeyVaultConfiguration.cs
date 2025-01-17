namespace Testcontainers.LowkeyVault;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class LowkeyVaultConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultConfiguration" /> class.
    /// </summary>
    /// <param name="vaultNames">The vault names.</param>
    /// <param name="aliasMap">The vault aliases.</param>
    /// <param name="externalConfigFilePath">The external config file path.</param>
    /// <param name="additionalArguments">The additional arguments to be passed via environment variables to the container.</param>
    public LowkeyVaultConfiguration(HashSet<string> vaultNames = null,
                                    Dictionary<string, HashSet<string>> aliasMap = null,
                                    string externalConfigFilePath = null,
                                    List<string> additionalArguments = null)
    {
        VaultNames = vaultNames;
        AliasMap = aliasMap;
        ExternalConfigFilePath = externalConfigFilePath;
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
        VaultNames = BuildConfiguration.Combine(oldValue.VaultNames, newValue.VaultNames);
        AliasMap = BuildConfiguration.Combine(oldValue.AliasMap, newValue.AliasMap);
        ExternalConfigFilePath = BuildConfiguration.Combine(oldValue.ExternalConfigFilePath, newValue.ExternalConfigFilePath);
        AdditionalArguments = BuildConfiguration.Combine(oldValue.AdditionalArguments, newValue.AdditionalArguments);
    }

    /// <summary>
    /// Gets the Vault names.
    /// </summary>
    internal HashSet<string> VaultNames { get; } = [];

    /// <summary>
    /// Gets the Alias Map.
    /// </summary>
    internal Dictionary<string, HashSet<string>> AliasMap { get; } = [];

    /// <summary>
    /// Gets the External Config File Path.
    /// </summary>
    internal string ExternalConfigFilePath { get; }

    /// <summary>
    /// Gets Additional Arguments.
    /// </summary>
    internal List<string> AdditionalArguments { get; } = [];
}