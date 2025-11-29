using DotNet.Testcontainers.Images;

namespace Testcontainers.RavenDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RavenDbBuilder : ContainerBuilder<RavenDbBuilder, RavenDbContainer, RavenDbConfiguration>
{
    public const string RavenDbImage = "ravendb/ravendb:5.4-ubuntu-latest";

    public const ushort RavenDbPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbBuilder" /> class.
    /// </summary>
    [Obsolete("Use constructor with image as a parameter instead.")]
    public RavenDbBuilder()
        : this(new RavenDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(RavenDbImage).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/r/ravendb/ravendb/tags">https://hub.docker.com/r/ravendb/ravendb/tags</see>.</param>
    public RavenDbBuilder(string image)
        : this(new RavenDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
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
            .WithImage(RavenDbImage)
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