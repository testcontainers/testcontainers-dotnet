using System.Net;

namespace Testcontainers.ClamAv;

[PublicAPI]
public sealed class ClamAvContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedisContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ClamAvContainer(ClamAvConfiguration configuration)
        : base(configuration)
    {
    }

    public DnsEndPoint GetDnsEndPoint()
    {
        return new DnsEndPoint(Hostname, GetMappedPublicPort(ClamAvBuilder.ClamAvPort));
    }
}
