using System.Threading;
using Health = Grpc.Health.V1.Health;

namespace Testcontainers.SpiceDB;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class SpiceDBContainer : DockerContainer
{
    SpiceDBConfiguration _configuration;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SpiceDBContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public SpiceDBContainer(SpiceDBConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    public string GetGrpcConnectionString()
    {
        var scheme = _configuration.TslEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        var endpoint = new UriBuilder(scheme, Hostname, GetMappedPublicPort(SpiceDBBuilder.SpiceDBgRPCPort));
        return endpoint.ToString();
    }
}
