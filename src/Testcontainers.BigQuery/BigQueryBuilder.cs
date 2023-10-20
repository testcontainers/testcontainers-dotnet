namespace Testcontainers.BigQuery;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class BigQueryBuilder : ContainerBuilder<BigQueryBuilder, BigQueryContainer, BigQueryConfiguration>
{
    public const string Image = "ghcr.io/goccy/bigquery-emulator";
    public const ushort BigQueryPort = 9050;
    private string DefaultProjectId = "test";
    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryBuilder" /> class.
    /// </summary>
    public BigQueryBuilder()
        : this(new BigQueryConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private BigQueryBuilder(BigQueryConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    protected override BigQueryConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override BigQueryContainer Build()
    {
        Validate();
        return new BigQueryContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override BigQueryBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new BigQueryConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override BigQueryBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new BigQueryConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override BigQueryBuilder Merge(BigQueryConfiguration oldValue, BigQueryConfiguration newValue)
    {
        return new BigQueryBuilder(new BigQueryConfiguration(oldValue, newValue));
    }
    
    public BigQueryBuilder WithProject(string project)
    {
        return Merge(DockerResourceConfiguration, new BigQueryConfiguration(project))
            .WithCommand("--project",project);
    }

    protected override BigQueryBuilder Init()
    {
        return base.Init()
            .WithProject(DefaultProjectId)
            .WithImage(Image)
            .WithPortBinding(BigQueryPort, true)
            .WithCommand("--project",DefaultProjectId)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*listening.*$"));
    }
}