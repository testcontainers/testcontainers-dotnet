using System.Threading;
using System.Threading.Tasks;
using Toxiproxy.Net;
using ToxiproxyNetClient = Toxiproxy.Net.Client;

namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ToxiproxyContainer : DockerContainer
{
    private readonly ToxiproxyConfiguration _configuration;

    private ToxiproxyNetClient _client;

    public ToxiproxyContainer(ToxiproxyConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Toxiproxy connection string.
    /// </summary>
    /// <returns>The Toxiproxy control endpoint URL.</returns>
    public string GetConnectionString()
    {
        return $"http://{Hostname}:{GetMappedPublicPort(ToxiproxyBuilder.ControlPort)}";
    }

    /// <summary>
    /// Starts the container and initializes the Toxiproxy client.
    /// </summary>
    public override async Task StartAsync(CancellationToken ct = default)
    {
        await base.StartAsync(ct);
        var connection = new Connection(Hostname, GetMappedPublicPort(ToxiproxyBuilder.ControlPort));
        _client = connection.Client();
    }

    public string GetHost()
    {
        return Hostname;
    }

    public int GetControlPort()
    {
        return GetMappedPublicPort(ToxiproxyBuilder.ControlPort);
    }
}