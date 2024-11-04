namespace Testcontainers.Playwright;

/// <inheritdoc cref="PlaywrightBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
/// <remarks>
/// Find further information about the Playwright image, here: https://playwright.dev/dotnet/docs/docker.
/// </remarks>
[PublicAPI]
public class PlaywrightBuilder : ContainerBuilder<PlaywrightBuilder, PlaywrightContainer, PlaywrightConfiguration>
{
  private const ushort PlaywrightPort = 53333;
  private const string PlaywrightEndpointPath = "/playwright";

  /// <summary>
  /// Initializes a new instance of the <see cref="PlaywrightBuilder" /> class.
  /// </summary>
  public PlaywrightBuilder() : this(new PlaywrightConfiguration())
  {
    DockerResourceConfiguration = Init().DockerResourceConfiguration;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="PlaywrightBuilder" /> class.
  /// </summary>
  /// <param name="resourceConfiguration">The Docker resource configuration.</param>
  private PlaywrightBuilder(PlaywrightConfiguration resourceConfiguration)
    : base(resourceConfiguration)
  {
    DockerResourceConfiguration = resourceConfiguration;
  }

  /// <inheritdoc />
  protected override PlaywrightConfiguration DockerResourceConfiguration { get; }

  public override PlaywrightContainer Build()
  {
    Validate();
    return new PlaywrightContainer(DockerResourceConfiguration);
  }

  /// <inheritdoc />
  protected override PlaywrightBuilder Init()
  {
    return base.Init()
      .WithBrowser(PlaywrightBrowser.Chrome)
      .WithEndpointPath(PlaywrightEndpointPath)
      .WithBrowserPort(PlaywrightPort)
      .WithWaitStrategy(Wait.ForUnixContainer()
        .UntilMessageIsLogged($"ws://.*:{PlaywrightPort}{PlaywrightEndpointPath}"));
  }

  public PlaywrightBuilder WithBrowser(PlaywrightBrowser playwrightBrowser)
  {
    return WithImage(playwrightBrowser.Image);
  }

  /// <summary>
  /// Sets the BROWSER WS ENDPOINT.
  /// </summary>
  /// <param name="endpointPath">The BROWSER WS ENDPOINT.</param>
  /// <returns>A configured instance of <see cref="PlaywrightBuilder" />.</returns>
  public PlaywrightBuilder WithEndpointPath(string endpointPath)
  {
    return Merge(DockerResourceConfiguration, new PlaywrightConfiguration(endpoint: endpointPath))
      .WithEnvironment("BROWSER_WS_ENDPOINT", endpointPath);
  }

  /// <summary>
  /// Sets the BROWSER WS PORT.
  /// </summary>
  /// <param name="port">The BROWSER WS PORT.</param>
  /// <returns>A configured instance of <see cref="PlaywrightBuilder" />.</returns>
  public PlaywrightBuilder WithBrowserPort(int port, bool assignRandomHostPort=false)
  {
    return Merge(DockerResourceConfiguration, new PlaywrightConfiguration(port: port))
      .WithEnvironment("BROWSER_PORT", port.ToString())
      .WithPortBinding(port, assignRandomHostPort);
  }



  /// <inheritdoc />
  protected override PlaywrightBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new PlaywrightConfiguration(resourceConfiguration));
  }

  /// <inheritdoc />
  protected override PlaywrightBuilder Clone(IContainerConfiguration resourceConfiguration)
  {
    return Merge(DockerResourceConfiguration, new PlaywrightConfiguration(resourceConfiguration));
  }

  /// <inheritdoc />
  protected override PlaywrightBuilder Merge(PlaywrightConfiguration oldValue, PlaywrightConfiguration newValue)
  {
    return new PlaywrightBuilder(new PlaywrightConfiguration(oldValue, newValue));
  }
}
