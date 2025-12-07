namespace Testcontainers.LowkeyVault;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class LowkeyVaultBuilder : ContainerBuilder<LowkeyVaultBuilder, LowkeyVaultContainer, LowkeyVaultConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string LowkeyVaultImage = "nagyesta/lowkey-vault:2.7.1-ubi9-minimal";

    public const ushort LowkeyVaultPort = 8443;

    public const ushort LowkeyVaultTokenPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public LowkeyVaultBuilder()
        : this(LowkeyVaultImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>nagyesta/lowkey-vault:2.7.1-ubi9-minimal</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/nagyesta/lowkey-vault/tags" />.
    /// </remarks>
    public LowkeyVaultBuilder(string image)
        : this(new LowkeyVaultConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/nagyesta/lowkey-vault/tags" />.
    /// </remarks>
    public LowkeyVaultBuilder(IImage image)
        : this(new LowkeyVaultConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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
    /// Collects and appends Lowkey Vault arguments to the container start.
    /// </summary>
    /// <remarks>
    /// The method adds the provided arguments to the <c>LOWKEY_ARGS</c> environment variable.
    /// E.g. <c>--LOWKEY_DEBUG_REQUEST_LOG=true</c>.
    /// </remarks>
    /// <param name="arguments">The arguments to add to the <c>LOWKEY_ARGS</c> environment variable.</param>
    /// <returns>A configured instance of <see cref="LowkeyVaultBuilder" />.</returns>
    public LowkeyVaultBuilder WithArguments(IEnumerable<string> arguments)
    {
        return Merge(DockerResourceConfiguration, new LowkeyVaultConfiguration(arguments: arguments));
    }

    /// <inheritdoc />
    public override LowkeyVaultContainer Build()
    {
        Validate();

        var lowkeyVaultBusBuilder = WithEnvironment("LOWKEY_ARGS", string.Join(" ", DockerResourceConfiguration.Arguments));
        return new LowkeyVaultContainer(lowkeyVaultBusBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override LowkeyVaultBuilder Init()
    {
        return base.Init()
            .WithPortBinding(LowkeyVaultPort, true)
            .WithPortBinding(LowkeyVaultTokenPort, true)
            .WithArguments(new[] { "--LOWKEY_VAULT_RELAXED_PORTS=true" })
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*Started LowkeyVaultApp.*$"));
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
}