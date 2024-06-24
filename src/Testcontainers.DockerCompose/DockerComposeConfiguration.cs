namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class DockerComposeConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeConfiguration" /> class.
    /// </summary>
    /// <param name="composeFilePath">The Docker Compose file path.</param>
    /// <param name="mode">The Docker Compose mode.</param>
    public DockerComposeConfiguration(
        string composeFilePath = null,
        DockerComposeMode? mode = null)
    {
        ComposeFilePath = composeFilePath;
        Mode = mode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DockerComposeConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DockerComposeConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DockerComposeConfiguration(DockerComposeConfiguration resourceConfiguration)
        : this(new DockerComposeConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public DockerComposeConfiguration(DockerComposeConfiguration oldValue, DockerComposeConfiguration newValue)
        : base(oldValue, newValue)
    {
        ComposeFilePath = BuildConfiguration.Combine(oldValue.ComposeFilePath, newValue.ComposeFilePath);
        Mode = BuildConfiguration.Combine(oldValue.Mode, newValue.Mode);
    }

    /// <summary>
    /// Gets the Docker Compose file path.
    /// </summary>
    public string ComposeFilePath { get; }

    /// <summary>
    /// Gets the Docker Compose mode.
    /// </summary>
    public DockerComposeMode? Mode { get; }
}