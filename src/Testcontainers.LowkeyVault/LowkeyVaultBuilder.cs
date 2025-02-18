namespace Testcontainers.LowkeyVault;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class LowkeyVaultBuilder : ContainerBuilder<LowkeyVaultBuilder, LowkeyVaultContainer, LowkeyVaultConfiguration>
{
    public const string LowkeyVaultImage = "nagyesta/lowkey-vault:2.7.1-ubi9-minimal";

    public const ushort LowkeyVaultPort = 8443;

    public const ushort LowkeyVaultTokenPort = 8080;

    public const string TokenEndPointPath = "/metadata/identity/oauth2/token";

    private const string LowKeyVaultEnvVarKey = "LOWKEY_ARGS";

    private readonly HashSet<string> NoAutoRegistration = ["-"];

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultBuilder" /> class.
    /// </summary>
    public LowkeyVaultBuilder()
        : this(new LowkeyVaultConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private LowkeyVaultBuilder(LowkeyVaultConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override LowkeyVaultConfiguration DockerResourceConfiguration { get; }


    /// <summary>
    /// Sets the vault names.
    /// </summary>
    /// <param name="vaultNames">The vault names.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithVaultNames(HashSet<string> vaultNames)
    {
        return Merge(DockerResourceConfiguration, new LowkeyVaultConfiguration(vaultNames: vaultNames))
            .WithEnvironment(LowKeyVaultEnvVarKey, AddOrAppend($"--LOWKEY_VAULT_NAMES={string.Join(",", vaultNames)}"));
    }

    /// <summary>
    /// Sets the vault aliases.
    /// </summary>
    /// <param name="aliasMap">The vault aliases.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithVaultAliases(Dictionary<string, HashSet<string>> aliasMap)
    {
        return Merge(DockerResourceConfiguration, new LowkeyVaultConfiguration(aliasMap: aliasMap))
            .WithEnvironment(LowKeyVaultEnvVarKey, AddOrAppend($"--LOWKEY_VAULT_ALIASES={ProcessVaultAliases(aliasMap)}"));
    }

    /// <summary>
    /// Sets No Auto Registration.
    /// </summary>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithNoAutoRegistration()
    {
        return WithVaultNames(NoAutoRegistration);
    }

    /// <summary>
    /// Sets Import file.
    /// </summary>
    /// <param name="importFilePath">The import file path.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithImportFile(string importFilePath)
    {
        return WithEnvironment(LowKeyVaultEnvVarKey, AddOrAppend($"--LOWKEY_IMPORT_LOCATION={importFilePath}"))
            .WithResourceMapping(new FileInfo(importFilePath), new FileInfo("/import/vaults.json"));
    }

    /// <summary>
    /// Sets External config file.
    /// </summary>
    /// <param name="externalConfigFilePath">The external config file path.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithExternalConfigFile(string externalConfigFilePath)
    {
        return Merge(DockerResourceConfiguration, new LowkeyVaultConfiguration(externalConfigFilePath: externalConfigFilePath))
            .WithResourceMapping(new FileInfo(externalConfigFilePath), new FileInfo("/config/application.properties"));
    }

    /// <summary>
    /// Sets Custom Ssl Certificate file.
    /// </summary>
    /// <param name="keyStoreFilePath">The keyStore (custom Ssl Certificate) file path.</param>
    /// <param name="keyStorePassword">The keyStore password.</param>
    /// <param name="keyStoreType">The keyStore Type.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithCustomSslCertificate(string keyStoreFilePath, string keyStorePassword, StoreType keyStoreType = StoreType.PKCS12)
    {
        return WithEnvironment(LowKeyVaultEnvVarKey, AddOrAppend($"--server.ssl.key-store={keyStoreFilePath} "
                                                               + $"--server.ssl.key-store-type={keyStoreType} "
                                                               + $"--server.ssl.key-store-password={keyStorePassword ?? string.Empty}"))
            .WithResourceMapping(new FileInfo(keyStoreFilePath), new FileInfo("/config/cert.store"));
    }

    /// <summary>
    /// Enable Or Disable Debug.
    /// </summary>
    /// <param name="debug">The flag to enable or disable debug.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithDebug(bool debug)
    {
        return WithEnvironment(LowKeyVaultEnvVarKey, AddOrAppend($"--LOWKEY_DEBUG_REQUEST_LOG={debug}"));
    }

    /// <summary>
    /// Enable Or Disable Relaxed Ports.
    /// </summary>
    /// <param name="relaxedPorts">The flag to enable or disable relaxed ports.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithRelaxedPorts(bool relaxedPorts)
    {
        return WithEnvironment(LowKeyVaultEnvVarKey, AddOrAppend($"--LOWKEY_VAULT_RELAXED_PORTS={relaxedPorts}"));
    }

    /// <summary>
    /// Sets The host used to replace host placeholder in import template.
    /// </summary>
    /// <param name="logicalHost">The logical host.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithLogicalHost(string logicalHost)
    {
        return WithEnvironment(LowKeyVaultEnvVarKey, AddOrAppend($"--LOWKEY_IMPORT_TEMPLATE_HOST={logicalHost}"));
    }

    /// <summary>
    /// Sets The port used to replace host placeholder in import template.
    /// </summary>
    /// <param name="logicalPort">The logical port.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithLogicalPort(ushort logicalPort)
    {
        return WithEnvironment(LowKeyVaultEnvVarKey, AddOrAppend($"--LOWKEY_IMPORT_TEMPLATE_PORT={logicalPort}"));
    }


    /// <summary>
    /// Sets Additional Arguments.
    /// </summary>
    /// <param name="debug">The additional arguments.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithAdditionalArguments(List<string> additionalArguments)
    {
        return Merge(DockerResourceConfiguration, new LowkeyVaultConfiguration(additionalArguments: additionalArguments))
            .WithEnvironment(LowKeyVaultEnvVarKey, AddOrAppend(string.Join(" ", additionalArguments)));
    }

    /// <inheritdoc />
    public override LowkeyVaultContainer Build()
    {
        Validate();

        var waitStrategy = Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*Started LowkeyVaultApp.*$");

        var lowkeyVaultBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(waitStrategy);

        return new LowkeyVaultContainer(lowkeyVaultBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override LowkeyVaultBuilder Init()
    {
        return base.Init()
            .WithImage(LowkeyVaultImage)
            .WithPortBinding(LowkeyVaultPort, true)
            .WithPortBinding(LowkeyVaultTokenPort, true)
            .WithRelaxedPorts(true);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.VaultNames, nameof(DockerResourceConfiguration.VaultNames)).NotNull();
        _ = Guard.Argument(DockerResourceConfiguration.AliasMap, nameof(DockerResourceConfiguration.AliasMap)).NotNull();
        _ = Guard.Argument(DockerResourceConfiguration.AdditionalArguments, nameof(DockerResourceConfiguration.AdditionalArguments)).NotNull();
        _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration.ExternalConfigFilePath))
                 .ThrowIf(argument =>
                 {
                     var externalConfigFilePath = argument.Value.ExternalConfigFilePath;
                     var fileName = Path.GetFileName(externalConfigFilePath);
                     return !string.IsNullOrEmpty(externalConfigFilePath) && !Path.GetFileName(fileName).EndsWith(".properties", StringComparison.Ordinal);
                 }, argument => throw new ArgumentException("External configuration file must be a *.properties file."));

        ValidateVaultNames(DockerResourceConfiguration.VaultNames);

        ValidateAliasMap(DockerResourceConfiguration.AliasMap);
    }

    /// <inheritdoc />
    protected override LowkeyVaultBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new LowkeyVaultConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override LowkeyVaultBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new LowkeyVaultConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override LowkeyVaultBuilder Merge(LowkeyVaultConfiguration oldValue, LowkeyVaultConfiguration newValue)
    {
        return new LowkeyVaultBuilder(new LowkeyVaultConfiguration(oldValue, newValue));
    }

    private string AddOrAppend(string value)
    {
        return DockerResourceConfiguration.Environments.TryGetValue(LowKeyVaultEnvVarKey, out var existingValue)
            ? MergeEnv(existingValue, value)
            : value;
    }

    /// <summary>
    /// Merges two input strings by treating strings starting with `--` as keys
    /// and strings after `=` as their corresponding values. Updates the original
    /// string with values from the new string if keys overlap.
    /// </summary>
    /// <param name="originalString">The original input string containing key-value pairs.</param>
    /// <param name="newString">The new input string containing updated key-value pairs.</param>
    /// <returns>The merged string with updated key-value pairs.</returns>
    private static string MergeEnv(string originalString, string newString)
    {
        var originalDict = ParseToDictionary(originalString);
        var newDict = ParseToDictionary(newString);

        foreach (var kvp in newDict)
            originalDict[kvp.Key] = kvp.Value;

        return string.Join(" ", originalDict.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        /// <summary>
        /// Parses an input string into a dictionary where keys are the parts starting with `--`
        /// and values are extracted from the segment after `=`. Handles multi-word values.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <returns>A dictionary representation of the key-value pairs in the input string.</returns>
        static Dictionary<string, string> ParseToDictionary(string input)
        {
            var dictionary = new Dictionary<string, string>();
            var parts = input.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            string currentKey = null;

            foreach (var part in parts)
            {
                if (part.StartsWith("--"))
                {
                    var equalIndex = part.IndexOf('=');

                    if (equalIndex > 0)
                    {
                        dictionary[part.Substring(0, equalIndex)] = part.Substring(equalIndex + 1);
                    }
                    else
                    {
                        currentKey = part;
                    }
                }
                else if (currentKey != null)
                {
                    dictionary[currentKey] = part;
                    currentKey = null;
                }
            }

            return dictionary;
        }
    }

    private void ValidateVaultNames(HashSet<string> vaultNames)
    {
        if (!NoAutoRegistration.SetEquals(vaultNames))
        {
            var invalid = vaultNames.Where(s => !Regex.IsMatch(s ?? string.Empty, "^[0-9a-zA-Z-]+$", RegexOptions.Compiled)).ToList();

            if (invalid.Count != 0)
            {
                throw new ArgumentException("VaultNames contains invalid values: " + string.Join(", ", invalid));
            }
        }
    }

    private static void ValidateAliasMap(Dictionary<string, HashSet<string>> aliasMap)
    {
        foreach (var host in aliasMap.Keys)
        {
            if (!Regex.IsMatch(host, "^[0-9a-z\\-_.]+$"))
            {
                throw new ArgumentException($"Vault host names must match '^[0-9a-z\\-_.]+$'. Found: {host}");
            }
        }

        foreach (var alias in aliasMap.Values)
        {
            foreach (var host in alias)
            {
                if (!Regex.IsMatch(host, "^[0-9a-z\\-_.]+(:[0-9]+|:<port>)?$"))
                {
                    throw new ArgumentException($"Vault aliases must match '^[0-9a-z\\-_.]+(:[0-9]+|:<port>)?$'. Found: {host}");
                }
            }
        }
    }

    private static string ProcessVaultAliases(Dictionary<string, HashSet<string>> aliasMap)
    {
        return aliasMap.OrderBy(pair => pair.Key) // Sort keys
                       .SelectMany(pair => pair.Value.OrderBy(alias => alias) // Sort values
                       .Select(alias => $"{pair.Key}={alias}"))
                       .Aggregate((current, next) => $"{current},{next}"); // Join the pairs into a single string with commas
    }

    public enum StoreType
    {
        JKS,
        PKCS12
    }
}