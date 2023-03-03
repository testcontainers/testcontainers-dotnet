namespace Testcontainers.WebDriver;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class WebDriverConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverConfiguration" /> class.
    /// </summary>
    /// /// <param name="browserType">The browser type running on.</param>
    public WebDriverConfiguration(string browserType = null)
    {
        BrowserType = browserType;
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
    }
    
    /// <summary>
    /// Gets the browser type running on.
    /// </summary>
    public string BrowserType { get; }
}