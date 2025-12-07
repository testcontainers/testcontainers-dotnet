namespace Testcontainers.BigQuery;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class BigQueryBuilder : ContainerBuilder<BigQueryBuilder, BigQueryContainer, BigQueryConfiguration>
{
    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string BigQueryImage = "ghcr.io/goccy/bigquery-emulator:0.4";

    public const ushort BigQueryPort = 9050;

    public const string DefaultProjectId = "default";

    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public BigQueryBuilder()
        : this(BigQueryImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>ghcr.io/goccy/bigquery-emulator:0.4</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://github.com/goccy/bigquery-emulator/pkgs/container/bigquery-emulator" />.
    /// </remarks>
    public BigQueryBuilder(string image)
        : this(new BigQueryConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigQueryBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://github.com/goccy/bigquery-emulator/pkgs/container/bigquery-emulator" />.
    /// </remarks>
    public BigQueryBuilder(IImage image)
        : this(new BigQueryConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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

    /// <inheritdoc />
    protected override BigQueryConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public BigQueryBuilder WithProject(string projectId)
    {
        return WithCommand("--project", projectId);
    }

    /// <inheritdoc />
    public override BigQueryContainer Build()
    {
        Validate();
        return new BigQueryContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override BigQueryBuilder Init()
    {
        return base.Init()
            .WithPortBinding(BigQueryPort, true)
            .WithProject(DefaultProjectId)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*listening.*$"));
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
}