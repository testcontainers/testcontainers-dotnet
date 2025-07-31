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
        var endpoint = new UriBuilder(scheme, Hostname, GetMappedPublicPort(SpiceDBBuilder.SpiceDBPort));
        return endpoint.ToString();
    }

    public GrpcChannel GetGrpcChannel()
    {
        return GrpcChannel.ForAddress(GetGrpcConnectionString());
    }

    public async Task<string> GetStateAsync(CancellationToken cancellationToken = default)
    {
        var healthClient = new Health.HealthClient(GetGrpcChannel());
        var response = await healthClient.CheckAsync(new HealthCheckRequest
        {
            Service = string.Empty, 
        }, null, null, cancellationToken);
        return response.Status.ToString();
    }
}
