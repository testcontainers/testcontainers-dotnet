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
    private readonly IEnumerable<Proxy> _initialProxies;
    private ToxiproxyNetClient? _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="initialProxies">Optional proxies to be created automatically after startup.</param>
    public ToxiproxyContainer(ToxiproxyConfiguration configuration, IEnumerable<Proxy>? initialProxies = null)
        : base(configuration)
    {
        _configuration = configuration;
        _initialProxies = initialProxies ?? Enumerable.Empty<Proxy>();
    }

    /// <summary>
    /// Gets the Toxiproxy client. Must call <see cref="StartAsync" /> before accessing.
    /// </summary>
    public ToxiproxyNetClient Client =>
        _client ?? throw new InvalidOperationException("Toxiproxy client is not initialized. Call StartAsync() first.");

    /// <summary>
    /// Gets the full URI of the Toxiproxy control endpoint.
    /// </summary>
    public Uri GetControlUri()
    {
        return new Uri($"http://{Hostname}:{GetMappedPublicPort(ToxiproxyBuilder.ControlPort)}");
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken ct = default)
    {
        await base.StartAsync(ct);
        
        try
        {
            var connection = new Connection(Hostname, GetMappedPublicPort(ToxiproxyBuilder.ControlPort));
            _client = connection.Client();

            foreach (var proxy in _initialProxies)
            {
                _client.Add(proxy);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize Toxiproxy client or create initial proxies.", ex);
        }
    }
}