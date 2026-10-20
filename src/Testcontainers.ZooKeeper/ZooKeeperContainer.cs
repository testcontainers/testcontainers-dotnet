namespace Testcontainers.ZooKeeper;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ZooKeeperContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ZooKeeperContainer(ZooKeeperConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the ZooKeeper connection string (a comma-free <c>host:port</c> pair as
    /// expected by ZooKeeper clients).
    /// </summary>
    /// <returns>The ZooKeeper connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder("tcp", Hostname, GetMappedPublicPort(ZooKeeperBuilder.ZooKeeperPort)).Uri.Authority;
    }
}
