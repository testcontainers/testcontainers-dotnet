namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class DockerComposeConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeConfiguration" /> class.
    /// </summary>
    /// <param name="composeFile">The fully qualified path to the compose file.</param>
    /// <param name="localCompose">Whether the local compose will be used.</param>
    /// <param name="options">Options for the docker-compose command.</param>
    /// <param name="removeImages">Options for remove images.</param>
    public DockerComposeConfiguration(
        string composeFile = null,
        bool localCompose = false, 
        IEnumerable<string> options = null, 
        RemoveImages removeImages = RemoveImages.None)
    {
        ComposeFile = composeFile;
        LocalCompose = localCompose;
        Options = options ?? Array.Empty<string>();
        RemoveImages = removeImages;
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
        ComposeFile = BuildConfiguration.Combine(oldValue.ComposeFile, newValue.ComposeFile);
        LocalCompose = BuildConfiguration.Combine(oldValue.LocalCompose, newValue.LocalCompose);
        RemoveImages = BuildConfiguration.Combine(oldValue.RemoveImages, newValue.RemoveImages);
    }
    
    /// <summary>
    /// Gets the path to the compose file.
    /// </summary>
    public string ComposeFile { get; }
    
    /// <summary>
    /// Indicates whether local compose is enabled.
    /// </summary>
    public bool LocalCompose { get; }

    /// <summary>
    /// Options for the docker-compose command.
    /// </summary>
    public IEnumerable<string> Options { get; } = Array.Empty<string>();
    
    /// <summary>
    /// Options for remove images.
    /// </summary>
    public RemoveImages RemoveImages { get; } = RemoveImages.None;
}