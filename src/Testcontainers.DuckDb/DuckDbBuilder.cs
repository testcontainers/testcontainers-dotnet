namespace Testcontainers.DuckDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DuckDbBuilder : ContainerBuilder<DuckDbBuilder, DuckDbContainer, DuckDbConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string DuckDbImage = "duckdb/duckdb:1.5.5";

    /// <summary>
    /// The path of the DuckDB CLI binary inside the container.
    /// </summary>
    public const string DuckDbBinaryFilePath = "/duckdb";

    /// <summary>
    /// The default path of the DuckDB database file inside the container.
    /// </summary>
    public const string DefaultDatabaseFilePath = "/database.duckdb";

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public DuckDbBuilder()
        : this(DuckDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>duckdb/duckdb:1.5.5</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/duckdb/duckdb/tags" />.
    /// </remarks>
    public DuckDbBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/duckdb/duckdb/tags" />.
    /// </remarks>
    public DuckDbBuilder(IImage image)
        : this(new DuckDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private DuckDbBuilder(DuckDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override DuckDbConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the path of the DuckDB database file inside the container.
    /// </summary>
    /// <remarks>
    /// DuckDB is an embedded database. The container keeps an in-memory DuckDB CLI
    /// process running to stay alive; SQL scripts run against the database file
    /// (created on first use) using <see cref="DuckDbContainer.ExecScriptAsync" />.
    /// </remarks>
    /// <param name="database">The DuckDB database file path.</param>
    /// <returns>A configured instance of <see cref="DuckDbBuilder" />.</returns>
    public DuckDbBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new DuckDbConfiguration(database: database));
    }

    /// <inheritdoc />
    public override DuckDbContainer Build()
    {
        Validate();
        return new DuckDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override DuckDbBuilder Init()
    {
        return base.Init()
            .WithEntrypoint(DuckDbBinaryFilePath)
            .WithCommand("-cmd", "SELECT 1;")
            .WithCreateParameterModifier(parameterModifier => parameterModifier.OpenStdin = true)
            .WithDatabase(DefaultDatabaseFilePath)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted(DuckDbBinaryFilePath, "-c", "SELECT 1;"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Database, nameof(DockerResourceConfiguration.Database))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override DuckDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DuckDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DuckDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DuckDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DuckDbBuilder Merge(DuckDbConfiguration oldValue, DuckDbConfiguration newValue)
    {
        return new DuckDbBuilder(new DuckDbConfiguration(oldValue, newValue));
    }
}
