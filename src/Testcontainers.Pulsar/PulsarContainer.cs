namespace Testcontainers.Pulsar;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class PulsarContainer : DockerContainer
{
    private readonly PulsarConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public PulsarContainer(PulsarConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Pulsar broker url.
    /// </summary>
    /// <returns>The Pulsar broker url.</returns>
    public string GetPulsarBrokerUrl() =>
        new UriBuilder("pulsar://", Hostname, GetMappedPublicPort(PulsarBuilder.PulsarBrokerPort)).ToString();

    /// <summary>
    /// Gets the Pulsar service url.
    /// </summary>
    /// <returns>The Pulsar service url.</returns>
    public string GetHttpServiceUrl() =>
        new UriBuilder("http", Hostname, GetMappedPublicPort(PulsarBuilder.PulsarBrokerHttpPort)).ToString();

    /// <summary>
    /// Creates Authentication token
    /// </summary>
    /// <param name="expiryTime">Relative expiry time for the token (eg: 1h, 3d, 10y)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Authentication token</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> CreateToken(TimeSpan expiryTime, CancellationToken cancellationToken = default)
    {
        if (_configuration.Authentication != PulsarBuilder.Enabled)
            throw new Exception($"Could not create the token, because WithAuthentication is not used.");

        var arguments = $"bin/pulsar tokens create --secret-key /pulsar/secret.key --subject test-user";

        if (expiryTime != Timeout.InfiniteTimeSpan)
            arguments += $" --expiry-time {expiryTime.TotalSeconds}s";

        var result = await ExecAsync(new[] { "/bin/bash", "-c", arguments }, cancellationToken);

        if (result.ExitCode != 0)
            throw new Exception($"Could not create the token: {result.Stderr}");

        return result.Stdout;
    }
}