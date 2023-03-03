namespace Testcontainers.WebDriver;

public readonly struct WebDriverType
{
    public static WebDriverType Chrome => new WebDriverType("chrome", new DockerImage("selenium/standalone-chrome"));
    public static WebDriverType Firefox => new WebDriverType("firefox", new DockerImage("selenium/standalone-firefox"));
    public static WebDriverType MicrosoftEdge => new WebDriverType("MicrosoftEdge", new DockerImage("selenium/standalone-edge"));
    public static WebDriverType Opera => new WebDriverType("opera", new DockerImage("selenium/standalone-opera"));
    public static WebDriverType Video => new WebDriverType("ffmpeg", new DockerImage("selenium/video"));

    private WebDriverType(string name, IImage image)
    {
        Name = name;
        Image = image;
    }

    public string Name { get; }

    public IImage Image { get; }
}