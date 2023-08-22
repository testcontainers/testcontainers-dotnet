namespace Testcontainers.Qdrant;
/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class QdrantBuilder : ContainerBuilder<QdrantBuilder, QdrantContainer, QdrantConfiguration>
{
    public const string QdrantImage = "qdrant/qdrant:latest";

    public const ushort QdrantPort = 6333;

    public const string QdrantContainerName = "qdrant";

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantBuilder" /> class.
    /// </summary>
    public QdrantBuilder() : this(new QdrantConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private QdrantBuilder(QdrantConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override QdrantConfiguration DockerResourceConfiguration { get; }


    /// <summary>
    /// Sets the Volume Mapping.
    /// <a href="https://docs.docker.com/storage/volumes/">Docker Volume</a>
    /// <a href="https://docs.docker.com/storage/bind-mounts/">Docker Bind Mount</a>
    /// <a href="https://hub.docker.com/r/qdrant/qdrant/">Qdrant</a>
    /// </summary>
    /// <param name="hostPath">The mapped Host Path .</param>
    /// <param name="containerPath">The mapped Container Path.</param>
    /// <param name="accessMode">The AccessMode.</param>
    /// <returns>A configured instance of <see cref="QdrantBuilder" />.</returns>
    public QdrantBuilder WithVolume(string hostPath, string containerPath, AccessMode accessMode)
    {
        return
            Merge(DockerResourceConfiguration,
                new QdrantConfiguration(hostPath: hostPath, containerPath: containerPath, accessMode: accessMode))
            ;
    }

    public QdrantBuilder WithContainerName(string containerName)
    {
        return
            Merge(DockerResourceConfiguration,
                new QdrantConfiguration(containerName: containerName))
            ;
    }

    /// <inheritdoc />
    public override QdrantContainer Build()
    {
        Validate();
        return new QdrantContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override QdrantBuilder Init()
    {
        var containerBuilder = base.Init()
            .WithImage(QdrantImage)
            .WithPortBinding(QdrantPort, true)
            .BuildWithContainerName(DockerResourceConfiguration.ContainerName)
            .BuildWithVolume(DockerResourceConfiguration.HostPath, DockerResourceConfiguration.ContainerPath, DockerResourceConfiguration.AccessMode);
        containerBuilder = containerBuilder.WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
        return containerBuilder;
    }

    /// <inheritdoc />
    protected override QdrantBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new QdrantConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override QdrantBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new QdrantConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override QdrantBuilder Merge(QdrantConfiguration oldValue, QdrantConfiguration newValue)
    {
        return new QdrantBuilder(new QdrantConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private static readonly string[] LineEndings = { "\r\n", "\n" };

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var (stdout, stderr) = await container.GetLogsAsync(timestampsEnabled: false)
                .ConfigureAwait(false);

            return 2.Equals(Array.Empty<string>()
                .Concat(stdout.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries))
                .Concat(stderr.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries))
                .Count(line => line.Contains("Access web UI at http")));
        }
    }
}

internal static class QdrantBuilderExtensions
{
    public static QdrantBuilder BuildWithContainerName(this QdrantBuilder builder, string containerName)
    {
        if (containerName is not null)
        {
            builder = builder.WithName(containerName);
        }
        return builder;
    }

    public static QdrantBuilder BuildWithVolume(this QdrantBuilder builder, string hostPath, string containerPath, AccessMode accessMode)
    {
        if (hostPath is not null)
        {
            builder = builder.WithBindMount(hostPath, containerPath, accessMode);
        }
        return builder;
    }
}
