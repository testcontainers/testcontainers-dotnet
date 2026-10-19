namespace Testcontainers.Vault;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class VaultBuilder : ContainerBuilder<VaultBuilder, VaultContainer, VaultConfiguration>
{
    public const ushort VaultPort = 8200;

    public const string DefaultRootToken = "root";

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>hashicorp/vault:2.2</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/hashicorp/vault/tags" />.
    /// </remarks>
    public VaultBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/hashicorp/vault/tags" />.
    /// </remarks>
    public VaultBuilder(IImage image)
        : this(new VaultConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private VaultBuilder(VaultConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override VaultConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Vault root token.
    /// </summary>
    /// <remarks>
    /// The Vault CLI inside the container uses the same token.
    /// </remarks>
    /// <param name="rootToken">The Vault root token.</param>
    /// <returns>A configured instance of <see cref="VaultBuilder" />.</returns>
    public VaultBuilder WithRootToken(string rootToken)
    {
        return Merge(DockerResourceConfiguration, new VaultConfiguration(rootToken: rootToken))
            .WithEnvironment("VAULT_DEV_ROOT_TOKEN_ID", rootToken)
            .WithEnvironment("VAULT_TOKEN", rootToken);
    }

    /// <summary>
    /// Adds Vault CLI commands that run after the container has started.
    /// </summary>
    /// <remarks>
    /// The commands run in order, without the leading <c>vault</c>, for example
    /// <c>secrets enable transit</c>. The container start fails if a command fails.
    /// </remarks>
    /// <param name="initCommands">The Vault CLI commands.</param>
    /// <returns>A configured instance of <see cref="VaultBuilder" />.</returns>
    public VaultBuilder WithInitCommand(params string[] initCommands)
    {
        return Merge(DockerResourceConfiguration, new VaultConfiguration(initCommands: initCommands));
    }

    /// <inheritdoc />
    public override VaultContainer Build()
    {
        Validate();
        return new VaultContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override VaultBuilder Init()
    {
        return base.Init()
            .WithPortBinding(VaultPort, true)
            .WithRootToken(DefaultRootToken)
            .WithEnvironment("VAULT_ADDR", "http://0.0.0.0:" + VaultPort)
            .WithCreateParameterModifier(parameterModifier => parameterModifier.HostConfig.CapAdd = new[] { "IPC_LOCK" })
            .WithConnectionStringProvider(() => new VaultConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/v1/sys/health").ForPort(VaultPort)))
            .WithStartupCallback(RunInitCommandsAsync);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string message = "The root token must not contain a period. Vault refuses such a token in dev mode and the container exits.";

        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.RootToken, nameof(DockerResourceConfiguration.RootToken))
            .NotNull()
            .NotEmpty()
            .ThrowIf(argument => argument.Value.IndexOf('.') >= 0, argument => new ArgumentException(message, argument.Name));
    }

    /// <inheritdoc />
    protected override VaultBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new VaultConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override VaultBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new VaultConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override VaultBuilder Merge(VaultConfiguration oldValue, VaultConfiguration newValue)
    {
        return new VaultBuilder(new VaultConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Runs the Vault CLI init commands inside the running container.
    /// </summary>
    /// <param name="container">The Vault container.</param>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when all init commands have succeeded.</returns>
    /// <exception cref="ExecFailedException">Thrown when an init command fails.</exception>
    private static async Task RunInitCommandsAsync(VaultContainer container, VaultConfiguration configuration, CancellationToken ct)
    {
        if (configuration.InitCommands.Count == 0)
        {
            return;
        }

        var initCommands = string.Join(" && ", configuration.InitCommands.Select(initCommand => "vault " + initCommand));

        _ = await container.ExecAsync(new[] { "/bin/sh", "-c", initCommands }, ct)
            .ThrowOnFailure()
            .ConfigureAwait(false);
    }
}
