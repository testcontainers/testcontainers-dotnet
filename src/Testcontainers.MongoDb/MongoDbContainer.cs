namespace Testcontainers.MongoDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MongoDbContainer : DockerContainer
{
    private readonly MongoDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MongoDbContainer(MongoDbConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MongoDb connection string.
    /// </summary>
    /// <returns>The MongoDb connection string.</returns>
    public string GetConnectionString()
    {
        // The MongoDb documentation recommends to use percent-encoding for username and password: https://www.mongodb.com/docs/manual/reference/connection-string/.
        var endpoint = new UriBuilder("mongodb", Hostname, GetMappedPublicPort(MongoDbBuilder.MongoDbPort));
        endpoint.UserName = Uri.EscapeDataString(_configuration.Username);
        endpoint.Password = Uri.EscapeDataString(_configuration.Password);
        endpoint.Query = "?directConnection=true";
        return endpoint.ToString();
    }

    /// <summary>
    /// Executes the JavaScript script in the MongoDb container.
    /// </summary>
    /// <param name="scriptContent">The content of the JavaScript script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the JavaScript script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        var whichMongoDbShell = await ExecAsync(new[] { "which", "mongosh" }, ct)
            .ConfigureAwait(false);

        var command = new[]
        {
            whichMongoDbShell.ExitCode == 0 ? "mongosh" : "mongo",
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