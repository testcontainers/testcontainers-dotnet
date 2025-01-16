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
    /// <param name="importFilePath">The import file path.</param>
    /// <param name="externalConfigFilePath">The external config file path.</param>
    /// <param name="debug">The debug flag.</param>
    /// <param name="logicalHost">The host used to replace host placeholder in import template.</param>
    /// <param name="logicalPort">The port used to replace port placeholder in import template.</param>
    /// <param name="additionalArguments">The additional arguments to be passed via environment variables to the container.</param>
    /// <param name="keyStoreFilePath">The keyStore (custom Ssl Certificate) file path.</param>
    /// <param name="keyStorePassword">The keyStore password.</param>
    /// <param name="keyStoreType">The keyStore type.</param>
    public LowkeyVaultConfiguration(HashSet<string> vaultNames = null,
                                    Dictionary<string, HashSet<string>> aliasMap = null,
                                    string importFilePath = null,
                                    string externalConfigFilePath = null,
                                    bool? debug = null,
                                    string logicalHost = null,
                                    ushort? logicalPort = null,
                                    List<string> additionalArguments = null,
                                    string keyStoreFilePath = null,
                                    string keyStorePassword = null,
                                    StoreType? keyStoreType = null)
    {
        VaultNames = vaultNames;
        AliasMap = aliasMap;
        ImportFilePath = importFilePath;
        ExternalConfigFilePath = externalConfigFilePath;
        Debug = debug;
        LogicalHost = logicalHost;
        LogicalPort = logicalPort;
        AdditionalArguments = additionalArguments;
        KeyStoreFilePath = keyStoreFilePath;
        KeyStorePassword = keyStorePassword;
        KeyStoreType = keyStoreType;
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
        ImportFilePath = BuildConfiguration.Combine(oldValue.ImportFilePath, newValue.ImportFilePath);
        ExternalConfigFilePath = BuildConfiguration.Combine(oldValue.ExternalConfigFilePath, newValue.ExternalConfigFilePath);
        Debug = BuildConfiguration.Combine(oldValue.Debug, newValue.Debug);
        LogicalHost = BuildConfiguration.Combine(oldValue.LogicalHost, newValue.LogicalHost);
        LogicalPort = BuildConfiguration.Combine(oldValue.LogicalPort, newValue.LogicalPort);
        AdditionalArguments = BuildConfiguration.Combine(oldValue.AdditionalArguments, newValue.AdditionalArguments);
        KeyStoreFilePath = BuildConfiguration.Combine(oldValue.KeyStoreFilePath, newValue.KeyStoreFilePath);
        KeyStorePassword = BuildConfiguration.Combine(oldValue.KeyStorePassword, newValue.KeyStorePassword);
        KeyStoreType = BuildConfiguration.Combine(oldValue.KeyStoreType, newValue.KeyStoreType);
    }

    /// <summary>
    /// Gets the Vault names.
    /// </summary>
    public HashSet<string> VaultNames { get; } = [];

    /// <summary>
    /// Gets the Alias Map.
    /// </summary>
    public Dictionary<string, HashSet<string>> AliasMap { get; } = [];

    /// <summary>
    /// Gets the Import File Path.
    /// </summary>
    public string ImportFilePath { get; }

    /// <summary>
    /// Gets the External Config File Path.
    /// </summary>
    public string ExternalConfigFilePath { get; }

    /// <summary>
    /// Gets Whether to Log Debug.
    /// </summary>
    public bool? Debug { get; }

    /// <summary>
    /// Gets Logical Host.
    /// </summary>
    public string LogicalHost { get; }

    /// <summary>
    /// Gets Logical Port.
    /// </summary>
    public ushort? LogicalPort { get; }

    /// <summary>
    /// Gets Additional Arguments.
    /// </summary>
    public List<string> AdditionalArguments { get; } = [];

    /// <summary>
    /// Gets KeyStore File Path.
    /// </summary>
    public string KeyStoreFilePath { get; }

    /// <summary>
    /// Gets KeyStore Password.
    /// </summary>
    public string KeyStorePassword { get; }

    /// <summary>
    /// Gets KeyStore Type.
    /// </summary>
    public StoreType? KeyStoreType { get; }
}

public enum StoreType
{
    JKS,
    PKCS12
}