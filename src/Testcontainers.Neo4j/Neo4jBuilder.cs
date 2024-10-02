namespace Testcontainers.Neo4j;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class Neo4jBuilder : ContainerBuilder<Neo4jBuilder, Neo4jContainer, Neo4jConfiguration>
{
    public const string Neo4jImage = "neo4j:5.4";

    public const string Neo4jEnterpriseImage = "neo4j:5.4-enterprise";

    public const ushort Neo4jHttpPort = 7474;

    public const ushort Neo4jBoltPort = 7687;

    private const string AcceptLicenseAgreementEnvVar = "NEO4J_ACCEPT_LICENSE_AGREEMENT";

    private const string AcceptLicenseAgreementEnvVarValue = "yes";

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jBuilder" /> class.
    /// </summary>
    public Neo4jBuilder()
        : this(new Neo4jConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private Neo4jBuilder(Neo4jConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override Neo4jConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override Neo4jContainer Build()
    {
        Validate();
        return new Neo4jContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        if (IsEnterpriseImage())
        {
            const string message = $"The 'enterprise' image requires setting environment variable {AcceptLicenseAgreementEnvVar} to= '{AcceptLicenseAgreementEnvVarValue}'";
            _ = Guard.Argument(DockerResourceConfiguration, "Enterprise License agreement")
                .ThrowIf(
                    argument => !argument.Value.Environments.TryGetValue(AcceptLicenseAgreementEnvVar, out var licenseAgreementValue) || licenseAgreementValue != AcceptLicenseAgreementEnvVarValue,
                    argument => new ArgumentException(message, argument.Name));
        }
    }

    private bool IsEnterpriseImage()
    {
        return DockerResourceConfiguration.Image.Tag?.Contains("enterprise") ?? false;
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Init()
    {
        return base.Init()
            .WithImage(Neo4jImage)
            .WithPortBinding(Neo4jHttpPort, true)
            .WithPortBinding(Neo4jBoltPort, true)
            .WithEnvironment("NEO4J_AUTH", "none")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(Neo4jHttpPort)));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Neo4jConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Neo4jConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Merge(Neo4jConfiguration oldValue, Neo4jConfiguration newValue)
    {
        return new Neo4jBuilder(new Neo4jConfiguration(oldValue, newValue));
    }

    public Neo4jBuilder WithEnterpriseEdition()
    {
        return WithImage(Neo4jEnterpriseImage)
            .WithEnvironment(AcceptLicenseAgreementEnvVar, AcceptLicenseAgreementEnvVarValue);
    }
}