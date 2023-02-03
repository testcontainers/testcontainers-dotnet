namespace Testcontainers.MsSql;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MsSqlContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public MsSqlContainer(MsSqlConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}