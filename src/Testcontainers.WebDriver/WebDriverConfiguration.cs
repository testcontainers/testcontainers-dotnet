namespace Testcontainers.WebDriver;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class WebDriverConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverConfiguration" /> class.
    /// </summary>
    /// <param name="network">The network.</param>
    /// <param name="ffmpegContainer">The ffmpeg container.</param>
    public WebDriverConfiguration(
        INetwork network = null,
        IContainer ffmpegContainer = null)
    {
        Network = network;
        FFmpegContainer = ffmpegContainer;
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
    public WebDriverConfiguration(WebDriverConfiguration oldValue, WebDriverConfiguration newValue) : base(oldValue,
        newValue)
    {
        Network = BuildConfiguration.Combine(oldValue.Network, newValue.Network);
        FFmpegContainer = BuildConfiguration.Combine(oldValue.FFmpegContainer, newValue.FFmpegContainer);
    }

    /// <summary>
    /// Gets the network.
    /// </summary>
    public INetwork Network { get; }

    /// <summary>
    /// Gets the ffmpeg container.
    /// </summary>
    public IContainer FFmpegContainer { get; }
}