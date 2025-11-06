namespace Testcontainers.Playwright;

/// <summary>
/// Playwright browser configuration.
/// </summary>
/// <remarks>
/// The browsers are standalone, pre-built Playwright versions from the
/// https://github.com/JacobLinCool/playwright-docker repository.
/// </remarks>
[PublicAPI]
public readonly struct PlaywrightBrowser
{
    /// <summary>
    /// Gets the Playwright standalone Chrome configuration.
    /// </summary>
    public static readonly PlaywrightBrowser Chrome = new PlaywrightBrowser("jacoblincool/playwright:chrome-server-1.55.1");

    /// <summary>
    /// Gets the Playwright standalone Chromium configuration.
    /// </summary>
    public static readonly PlaywrightBrowser Chromium = new PlaywrightBrowser("jacoblincool/playwright:chromium-server-1.55.1");

    /// <summary>
    /// Gets the Playwright standalone Firefox configuration.
    /// </summary>
    public static readonly PlaywrightBrowser Firefox = new PlaywrightBrowser("jacoblincool/playwright:firefox-server-1.55.1");

    /// <summary>
    /// Gets the Playwright standalone Edge configuration.
    /// </summary>
    public static readonly PlaywrightBrowser Edge = new PlaywrightBrowser("jacoblincool/playwright:msedge-server-1.55.1");

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaywrightBrowser" /> struct.
    /// </summary>
    /// <param name="image">The Playwright standalone Docker image.</param>
    public PlaywrightBrowser(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaywrightBrowser" /> struct.
    /// </summary>
    /// <param name="image">The Playwright standalone Docker image.</param>
    public PlaywrightBrowser(IImage image)
    {
        Image = image;
    }

    /// <summary>
    /// Gets the Playwright standalone Docker image.
    /// </summary>
    [NotNull]
    public IImage Image { get; }
}