namespace Testcontainers.WireMock;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class WireMockContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public WireMockContainer(WireMockConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the WireMock base URL.
    /// </summary>
    /// <returns>The WireMock base URL.</returns>
    public string GetBaseUrl()
    {
        return new UriBuilder("http", Hostname, GetMappedPublicPort(WireMockBuilder.WireMockPort)).ToString();
    }

    /// <summary>
    /// Gets the WireMock admin URL.
    /// </summary>
    /// <returns>The WireMock admin URL.</returns>
    public string GetAdminUrl()
    {
        return $"{GetBaseUrl()}__admin";
    }

    /// <summary>
    /// Adds a mapping from a JSON string.
    /// </summary>
    /// <param name="mappingJson">The mapping JSON content.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the mapping has been added.</returns>
    public async Task AddMappingFromJsonAsync(string mappingJson, CancellationToken ct = default)
    {
        var mappingFilePath = $"/home/wiremock/mappings/{Guid.NewGuid():N}.json";
        
        await CopyAsync(Encoding.UTF8.GetBytes(mappingJson), mappingFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Adds a static file to be served by WireMock.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="fileContent">The file content.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the file has been added.</returns>
    public async Task AddStaticFileAsync(string fileName, byte[] fileContent, CancellationToken ct = default)
    {
        var filePath = $"/home/wiremock/__files/{fileName}";
        
        await CopyAsync(fileContent, filePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Resets all mappings and request logs.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when WireMock has been reset.</returns>
    public async Task<ExecResult> ResetAsync(CancellationToken ct = default)
    {
        return await ExecAsync(new[] { "curl", "-X", "POST", "http://localhost:8080/__admin/reset" }, ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the number of requests received.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The execution result containing the request count.</returns>
    public async Task<ExecResult> GetRequestCountAsync(CancellationToken ct = default)
    {
        return await ExecAsync(new[] { "curl", "-s", "http://localhost:8080/__admin/requests/count" }, ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all received requests.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The execution result containing the requests.</returns>
    public async Task<ExecResult> GetRequestsAsync(CancellationToken ct = default)
    {
        return await ExecAsync(new[] { "curl", "-s", "http://localhost:8080/__admin/requests" }, ct)
            .ConfigureAwait(false);
    }
}