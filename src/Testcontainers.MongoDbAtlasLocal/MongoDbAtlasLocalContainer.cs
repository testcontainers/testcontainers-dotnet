namespace Testcontainers.MongoDbAtlasLocal;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MongoDbAtlasLocalContainer : DockerContainer
{
    private const string MongoDbShellFilePath = "mongosh";

    private readonly MongoDbAtlasLocalConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MongoDbAtlasLocalContainer(MongoDbAtlasLocalConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MongoDb Atlas Local connection string.
    /// </summary>
    /// <returns>The MongoDb Atlas Local connection string.</returns>
    public string GetConnectionString()
    {
        // The MongoDb documentation recommends to use percent-encoding for username and password: https://www.mongodb.com/docs/manual/reference/connection-string/.
        var endpoint = new UriBuilder("mongodb", Hostname, GetMappedPublicPort(MongoDbAtlasLocalBuilder.MongoDbAtlasLocalPort));
        endpoint.UserName = Uri.EscapeDataString(_configuration.Username);
        endpoint.Password = Uri.EscapeDataString(_configuration.Password);
        endpoint.Query = "directConnection=true";
        return endpoint.ToString();
    }

    /// <summary>
    /// Executes the JavaScript script in the MongoDb Atlas Local container.
    /// </summary>
    /// <remarks>
    /// The script content is encoded using <see cref="Encoding.Default" />.
    /// </remarks>
    /// <param name="scriptContent">The content of the JavaScript script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the JavaScript script has been executed.</returns>
    public Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        return ExecScriptAsync(scriptContent, Encoding.Default, ct);
    }

    /// <summary>
    /// Executes the JavaScript script in the MongoDb Atlas Local container.
    /// </summary>
    /// <param name="scriptContent">The content of the JavaScript script to execute.</param>
    /// <param name="encoding">The encoding used to write the script content to the container.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the JavaScript script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, Encoding encoding, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(encoding.GetBytes(scriptContent), scriptFilePath, fileMode: Unix.FileMode644, ct: ct)
            .ConfigureAwait(false);

        var command = new[]
        {
            MongoDbShellFilePath,
            "--username", _configuration.Username,
            "--password", _configuration.Password,
            "--quiet",
            "--eval",
            $"load('{scriptFilePath}')",
        };

        return await ExecAsync(command, ct)
            .ConfigureAwait(false);
    }
}