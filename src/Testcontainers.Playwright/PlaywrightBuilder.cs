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
      .WithNetwork(new NetworkBuilder().Build())
      .WithEndpointPath(PlaywrightEndpointPath)
      .WithPortBinding(PlaywrightPort, true);
  }

  public PlaywrightBuilder WithBrowser(PlaywrightBrowser playwrightBrowser)
  {
    return WithImage(playwrightBrowser.Image);
  }

  /// <summary>
  /// Sets the MsSql password.
  /// </summary>
  /// <param name="password">The MsSql password.</param>
  /// <returns>A configured instance of <see cref="MsSqlBuilder" />.</returns>
  public PlaywrightBuilder WithEndpointPath(string endpointPath)
  {
    return Merge(DockerResourceConfiguration, new PlaywrightConfiguration(endpoint: endpointPath))
      .WithEnvironment("BROWSER_WS_ENDPOINT", endpointPath);
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
