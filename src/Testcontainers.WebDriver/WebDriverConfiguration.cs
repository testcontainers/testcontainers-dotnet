namespace Testcontainers.WebDriver;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class WebDriverConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverConfiguration" /> class.
    /// </summary>
    /// <param name="network">The network for recording.</param>
    /// <param name="recordingContainer">The ffmpeg video recording container.</param>
    public WebDriverConfiguration(
        INetwork network = null,
        IContainer recordingContainer = null)
    {
        Network = network;
        RecordingContainer = recordingContainer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public WebDriverConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public WebDriverConfiguration(IContainerConfiguration resourceConfiguration) : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public WebDriverConfiguration(WebDriverConfiguration resourceConfiguration)
        : this(new WebDriverConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public WebDriverConfiguration(WebDriverConfiguration oldValue, WebDriverConfiguration newValue) : base(oldValue, newValue)
    {
        Network = BuildConfiguration.Combine(oldValue.Network, newValue.Network);
        RecordingContainer = BuildConfiguration.Combine(oldValue.RecordingContainer, newValue.RecordingContainer);
    }

    /// <summary>
    /// Gets the shared recording network.
    /// </summary>
    public INetwork Network { get; }

    /// <summary>
    /// Gets the ffmpeg video recording container.
    /// </summary>
    public IContainer RecordingContainer { get; }
}