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
        return new LowkeyVaultContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override LowkeyVaultBuilder Init()
    {
        return base.Init()
            .WithImage(LowkeyVaultImage)
            .WithPortBinding(LowkeyVaultPort, true)
            .WithPortBinding(LowkeyVaultTokenPort, true)
            .WithAdditionalArguments([$"--LOWKEY_VAULT_RELAXED_PORTS={true}"])
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*Started LowkeyVaultApp.*$"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();
        _ = Guard.Argument(DockerResourceConfiguration.AdditionalArguments, nameof(DockerResourceConfiguration.AdditionalArguments)).NotNull();
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
}