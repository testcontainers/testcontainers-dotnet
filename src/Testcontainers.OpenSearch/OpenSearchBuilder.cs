namespace Testcontainers.OpenSearch;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OpenSearchBuilder : ContainerBuilder<OpenSearchBuilder, OpenSearchContainer, OpenSearchConfiguration>
{
    public const string DefaultUsername = "admin";

    public const string DefaultPassword = "VeryStrongP@ssw0rd!";

    public const string OpenSearchImage = "opensearchproject/opensearch:2.12.0";

    public const ushort OpenSearchApiPort = 9200;

    public const ushort OpenSearchPerfAnalyzerPort = 9600;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchBuilder" /> class.
    /// </summary>
    public OpenSearchBuilder()
        : this(new OpenSearchConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private OpenSearchBuilder(OpenSearchConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    protected override OpenSearchConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the password for 'admin' user.
    /// </summary>
    /// <param name="password">Password requires a minimum of 8 characters and must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.</param>
    /// <returns>A configured instance of <see cref="OpenSearchBuilder" />.</returns>
    public OpenSearchBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(password: password))
            .WithEnvironment("OPENSEARCH_INITIAL_ADMIN_PASSWORD", password);
    }

    /// <inheritdoc />
    public override OpenSearchContainer Build()
    {
        Validate();
        return new OpenSearchContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Init()
    {
        return base.Init()
            .WithImage(OpenSearchImage)
            .WithPortBinding(OpenSearchApiPort, true)
            .WithPortBinding(OpenSearchPerfAnalyzerPort, true)
            .WithPassword(DefaultPassword)
            .WithEnvironment("discovery.type", "single-node")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Node '.*' initialized"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OpenSearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OpenSearchBuilder Merge(OpenSearchConfiguration oldValue, OpenSearchConfiguration newValue)
    {
        return new OpenSearchBuilder(new OpenSearchConfiguration(oldValue, newValue));
    }
}