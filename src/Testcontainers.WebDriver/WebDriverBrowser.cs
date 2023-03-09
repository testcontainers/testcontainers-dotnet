﻿namespace Testcontainers.WebDriver;

/// <summary>
/// Web Driver browser configuration.
/// </summary>
[PublicAPI]
public readonly struct WebDriverBrowser
{
    /// <summary>
    /// Gets the Selenium standalone Chrome configuration.
    /// </summary>
    public static WebDriverBrowser Chrome = new WebDriverBrowser(new DockerImage("selenium/standalone-chrome:110.0"));

    /// <summary>
    /// Gets the Selenium standalone Firefox configuration.
    /// </summary>
    public static WebDriverBrowser Firefox = new WebDriverBrowser(new DockerImage("selenium/standalone-firefox:110.0"));

    /// <summary>
    /// Gets the Selenium standalone Edge configuration.
    /// </summary>
    public static WebDriverBrowser Edge = new WebDriverBrowser(new DockerImage("selenium/standalone-edge:110.0"));

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBrowser" /> struct.
    /// </summary>
    /// <param name="image">The Selenium standalone Docker image.</param>
    private WebDriverBrowser(IImage image)
    {
        Image = image;
    }

    /// <summary>
    /// Gets the Selenium standalone Docker image.
    /// </summary>
    [NotNull]
    public IImage Image { get; }
}