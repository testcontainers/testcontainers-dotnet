namespace Testcontainers.WebDriver;

/// <summary>
/// Web Driver browser configuration.
/// </summary>
[PublicAPI]
public readonly struct WebDriverBrowser
{
    /// <summary>
    /// Gets the Selenium standalone Chrome configuration.
    /// </summary>
    public static readonly WebDriverBrowser Chrome = new WebDriverBrowser("selenium/standalone-chrome:110.0");

    /// <summary>
    /// Gets the Selenium standalone Firefox configuration.
    /// </summary>
    public static readonly WebDriverBrowser Firefox = new WebDriverBrowser("selenium/standalone-firefox:110.0");

    /// <summary>
    /// Gets the Selenium standalone Edge configuration.
    /// </summary>
    public static readonly WebDriverBrowser Edge = new WebDriverBrowser("selenium/standalone-edge:110.0");

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBrowser" /> struct.
    /// </summary>
    /// <param name="image">The Selenium standalone Docker image.</param>
    public WebDriverBrowser(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBrowser" /> struct.
    /// </summary>
    /// <param name="image">The Selenium standalone Docker image.</param>
    public WebDriverBrowser(IImage image)
    {
        Image = image;
    }

    /// <summary>
    /// Gets the Selenium standalone Docker image.
    /// </summary>
    [NotNull]
    public IImage Image { get; }
}