namespace Testcontainers.Dapr;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class DaprContainer : DockerContainer
{
    private readonly DaprConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="Daprontainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DaprContainer(DaprConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    public int DaprHttpPort
    {
        get { return GetMappedPublicPort(DaprBuilder.DaprHttpPort); }
    }

    public int DaprGrpcPort
    {
        get { return GetMappedPublicPort(DaprBuilder.DaprGrpcPort); }
    }
}