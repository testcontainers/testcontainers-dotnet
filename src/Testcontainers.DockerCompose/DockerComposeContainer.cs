namespace Testcontainers.DockerCompose;

[PublicAPI]
public class DockerComposeContainer : DockerContainer
{
    private readonly IContainer _proxyContainer;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeContainer" /> class. 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    public DockerComposeContainer(DockerComposeConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
        _proxyContainer = configuration.LocalCompose
            ? new DockerComposeLocal(configuration, logger)
            : new DockerComposeRemote(configuration, logger);
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken ct = default)
    {
        await _proxyContainer.StartAsync(ct);
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken ct = default)
    {
        await _proxyContainer.StopAsync(ct);
    }
}