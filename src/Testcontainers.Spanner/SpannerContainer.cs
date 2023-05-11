using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testcontainers.Spanner;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class SpannerContainer : DockerContainer
{
  private const string EnvironmentVariableEmulatorHost = "SPANNER_EMULATOR_HOST";
  private readonly SpannerConfiguration _configuration;

  public int GrpcPort => GetMappedPublicPort(SpannerBuilder.InternalGrpcPort);
  public int RestPort => GetMappedPublicPort(SpannerBuilder.InternalRestPort);

  /// <summary>
  /// Initializes a new instance of the <see cref="SpannerContainer" /> class.
  /// </summary>
  /// <param name="configuration">The container configuration.</param>
  /// <param name="logger">The logger.</param>
  public SpannerContainer(SpannerConfiguration configuration, ILogger logger)
    : base(configuration, logger)
  {
    _configuration = configuration;
  }

  public override async Task StartAsync(CancellationToken ct = default)
  {
    await base.StartAsync(ct);

    Environment.SetEnvironmentVariable(EnvironmentVariableEmulatorHost, $"{Hostname}:{GrpcPort}");
  }
}