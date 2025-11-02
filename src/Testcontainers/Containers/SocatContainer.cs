namespace DotNet.Testcontainers.Containers
{
  using JetBrains.Annotations;

  /// <inheritdoc cref="DockerContainer" />
  [PublicAPI]
  public sealed class SocatContainer : DockerContainer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SocatContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public SocatContainer(SocatConfiguration configuration)
      : base(configuration)
    {
    }
  }
}
