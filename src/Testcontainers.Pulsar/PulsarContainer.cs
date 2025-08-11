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
    public PulsarContainer(PulsarConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Pulsar broker address.
    /// </summary>
    /// <returns>The Pulsar broker address.</returns>
    public string GetBrokerAddress()
    {
        return new UriBuilder("pulsar", Hostname, GetMappedPublicPort(PulsarBuilder.PulsarBrokerDataPort)).ToString();
    }

    /// <summary>
    /// Gets the Pulsar web service address.
    /// </summary>
    /// <returns>The Pulsar web service address.</returns>
    public string GetServiceAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(PulsarBuilder.PulsarWebServicePort)).ToString();
    }

    /// <summary>
    /// Creates an authentication token.
    /// </summary>
    /// <param name="expiryTime">The time after the authentication token expires.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the authentication token has been created.</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<string> CreateAuthenticationTokenAsync(TimeSpan expiryTime, CancellationToken ct = default)
    {
        if (_configuration.AuthenticationEnabled.HasValue && !_configuration.AuthenticationEnabled.Value)
        {
            throw new ArgumentException("Failed to create token. Authentication is not enabled.");
        }

        var command = new List<string>
        {
            "bin/pulsar",
            "tokens",
            "create",
            "--secret-key",
            PulsarBuilder.SecretKeyFilePath,
            "--subject",
            PulsarBuilder.Username,
        };

        if (!Timeout.InfiniteTimeSpan.Equals(expiryTime))
        {
            int secondsToMilliseconds;

            if (_configuration.Image.MatchVersion(IsVersionAffectedByGhIssue22811))
            {
                Logger.LogWarning("The 'apachepulsar/pulsar:3.2.0-3' and '3.3.0' images contains a regression. The expiry time is converted to the wrong unit of time: https://github.com/apache/pulsar/issues/22811.");
                secondsToMilliseconds = 1000;
            }
            else
            {
                secondsToMilliseconds = 1;
            }

            command.Add("--expiry-time");
            command.Add($"{secondsToMilliseconds * expiryTime.TotalSeconds}s");
        }

        var tokensResult = await ExecAsync(command, ct)
            .ConfigureAwait(false);

        if (tokensResult.ExitCode != 0)
        {
            throw new ArgumentException($"Failed to create token. Command returned a non-zero exit code: {tokensResult.Stderr}.");
        }

        return tokensResult.Stdout;
    }

    /// <summary>
    /// Copies the Pulsar startup script to the container.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the startup script has been copied.</returns>
    internal Task CopyStartupScriptAsync(CancellationToken ct = default)
    {
        var startupScript = new StringWriter();
        startupScript.NewLine = "\n";
        startupScript.WriteLine("#!/bin/bash");

        if (_configuration.AuthenticationEnabled.HasValue && _configuration.AuthenticationEnabled.Value)
        {
            startupScript.WriteLine("bin/pulsar tokens create-secret-key --output " + PulsarBuilder.SecretKeyFilePath);
            startupScript.WriteLine("export brokerClientAuthenticationParameters=token:$(bin/pulsar tokens create --secret-key $PULSAR_PREFIX_tokenSecretKey --subject $superUserRoles)");
            startupScript.WriteLine("export CLIENT_PREFIX_authParams=$brokerClientAuthenticationParameters");
            startupScript.WriteLine("bin/apply-config-from-env.py conf/standalone.conf");
            startupScript.WriteLine("bin/apply-config-from-env.py --prefix CLIENT_PREFIX_ conf/client.conf");
        }

        startupScript.Write("bin/pulsar standalone");

        if (_configuration.FunctionsWorkerEnabled.HasValue && !_configuration.FunctionsWorkerEnabled.Value)
        {
            startupScript.Write(" --no-functions-worker");
            startupScript.Write(" --no-stream-storage");
        }

        return CopyAsync(Encoding.Default.GetBytes(startupScript.ToString()), PulsarBuilder.StartupScriptFilePath, Unix.FileMode755, ct);
    }

    private static bool IsVersionAffectedByGhIssue22811(System.Version version)
    {
        return version.Major == 3 && ((version.Minor == 2 && version.Build is >= 0 and <= 3) || (version.Minor == 3 && version.Build == 0));
    }
}