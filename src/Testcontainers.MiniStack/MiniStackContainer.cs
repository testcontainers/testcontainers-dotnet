namespace Testcontainers.MiniStack;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MiniStackContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MiniStackContainer(MiniStackConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the MiniStack connection string.
    /// </summary>
    /// <returns>The MiniStack connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MiniStackBuilder.MiniStackPort)).ToString();
    }

    /// <summary>
    /// Resets all MiniStack service state back to empty.
    /// Useful between test runs to get a clean environment without restarting the container.
    /// </summary>
    /// <param name="runInitScripts">
    /// When <c>true</c>, re-runs boot.d and ready.d init scripts after the reset,
    /// restoring any resources they create (queues, seed data, VPCs, etc.).
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the reset has finished.</returns>
    public async Task ResetAsync(bool runInitScripts = false, CancellationToken ct = default)
    {
        var path = runInitScripts ? "/_ministack/reset?init=1" : "/_ministack/reset";
        var uri = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MiniStackBuilder.MiniStackPort), path).Uri;
        using var httpClient = new HttpClient();
        using var response = await httpClient.PostAsync(uri, content: null, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Returns all emails sent via SES, grouped by account.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The raw JSON response from <c>/_ministack/ses/messages</c>.</returns>
    public async Task<string> GetSesMessagesAsync(CancellationToken ct = default)
    {
        var uri = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MiniStackBuilder.MiniStackPort), "/_ministack/ses/messages").Uri;
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(uri, ct).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Returns all SQS messages across every queue, grouped by account.
    /// Messages are not consumed — they remain visible in the queue.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The raw JSON response from <c>/_ministack/sqs/messages</c>.</returns>
    public async Task<string> GetSqsMessagesAsync(CancellationToken ct = default)
    {
        var uri = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MiniStackBuilder.MiniStackPort), "/_ministack/sqs/messages").Uri;
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(uri, ct).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}
