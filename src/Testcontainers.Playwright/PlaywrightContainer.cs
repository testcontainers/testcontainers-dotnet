namespace Testcontainers.Playwright;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public class PlaywrightContainer : DockerContainer
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PlaywrightContainer" /> class.
  /// </summary>
  /// <param name="configuration">The container configuration.</param>
  public PlaywrightContainer(PlaywrightConfiguration configuration) : base(configuration)
  {
  }
}
