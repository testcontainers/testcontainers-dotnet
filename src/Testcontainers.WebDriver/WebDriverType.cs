namespace Testcontainers.WebDriver;

public readonly struct WebDriverType
{
    public static WebDriverType Chrome => new WebDriverType("chrome", new DockerImage("selenium/standalone-chrome:4.8.0"));
    public static WebDriverType Firefox => new WebDriverType("firefox", new DockerImage("selenium/standalone-firefox:4.8.0"));
    public static WebDriverType MicrosoftEdge => new WebDriverType("MicrosoftEdge", new DockerImage("selenium/standalone-edge:4.8.0"));
    public static WebDriverType Video => new WebDriverType("ffmpeg", new DockerImage("selenium/video:ffmpeg-4.3.1-20230221"));

    public WebDriverType(string name, IImage image)
    {
        Name = name;
        Image = image;
    }

    public string Name { get; }

    public IImage Image { get; }
}