namespace DotNet.Testcontainers.Containers
{
  using JetBrains.Annotations;

  /// <inheritdoc cref="DockerContainer" />
  [PublicAPI]
  public sealed class SocatContainer : DockerContainer
  {
    private readonly SocatConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SocatContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public SocatContainer(SocatConfiguration configuration)
      : base(configuration)
    {
      _configuration = configuration;
    }
  }
}
