namespace DotNet.Testcontainers.Images
{
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="FutureDockerImage" />
  /// <remarks>
  /// Builds the Docker image with BuildKit instead of the Docker Engine API, which
  /// does not support it.
  /// </remarks>
  [PublicAPI]
  internal sealed class BuildKitDockerImage : FutureDockerImage
  {
    private readonly IBuildKitImageFromDockerfileConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitDockerImage" /> class.
    /// </summary>
    /// <param name="configuration">The image configuration.</param>
    public BuildKitDockerImage(IBuildKitImageFromDockerfileConfiguration configuration)
      : base(configuration)
    {
      _configuration = configuration;
    }

    /// <inheritdoc />
    protected override Task BuildAsync(CancellationToken ct = default)
    {
      return Client.BuildAsync(_configuration, ct);
    }
  }
}
