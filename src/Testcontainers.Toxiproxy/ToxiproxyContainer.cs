namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ToxiproxyContainer : DockerContainer
{
    private readonly ToxiproxyConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ToxiproxyContainer(ToxiproxyConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }
}