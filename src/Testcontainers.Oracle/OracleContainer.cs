namespace Testcontainers.Oracle;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class OracleContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OracleContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public OracleContainer(OracleConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}