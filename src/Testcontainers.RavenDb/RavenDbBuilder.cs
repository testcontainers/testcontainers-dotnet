namespace Testcontainers.RavenDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RavenDbBuilder : ContainerBuilder<RavenDbBuilder, RavenDbContainer, RavenDbConfiguration>
{
    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string RavenDbImage = "ravendb/ravendb:5.4-ubuntu-latest";

    public const ushort RavenDbPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public RavenDbBuilder()
        : this(RavenDbImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>ravendb/ravendb:5.4-ubuntu-latest</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/ravendb/ravendb/tags" />.
    /// </remarks>
    public RavenDbBuilder(string image)
        : this(new RavenDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/ravendb/ravendb/tags" />.
    /// </remarks>
    public RavenDbBuilder(IImage image)
        : this(new RavenDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private RavenDbBuilder(RavenDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override RavenDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override RavenDbContainer Build()
    {
        Validate();
        return new RavenDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override RavenDbBuilder Init()
    {
        return base.Init()
            .WithPortBinding(RavenDbPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server started"));
    }

    /// <inheritdoc />
    protected override RavenDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RavenDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RavenDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RavenDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RavenDbBuilder Merge(RavenDbConfiguration oldValue, RavenDbConfiguration newValue)
    {
        return new RavenDbBuilder(new RavenDbConfiguration(oldValue, newValue));
    }
}