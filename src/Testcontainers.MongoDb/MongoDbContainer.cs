namespace Testcontainers.MongoDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MongoDbContainer : DockerContainer
{
    private static readonly string[] FindMongoDbShellFilePath = { "/bin/sh", "-c", "command -v mongosh || command -v mongo" };

    private readonly Lazy<Task<string>> _lazyMongoDbShellFilePath;

    private readonly MongoDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MongoDbContainer(MongoDbConfiguration configuration)
        : base(configuration)
    {
        _lazyMongoDbShellFilePath = new Lazy<Task<string>>(FindMongoDbShellFilePathAsync);
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
    /// Gets the MongoDb shell file path.
    /// </summary>
    /// <remarks>
    /// The file path represents the path from the container, not from the Docker or test host.
    /// Resolves <c>mongosh</c>, falling back to the legacy <c>mongo</c> shell that ships in
    /// images prior to MongoDB 6.0. The result is resolved once and reused.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the MongoDb shell file path has been found.</returns>
    public Task<string> GetMongoDbShellFilePathAsync(CancellationToken ct = default)
    {
        return _lazyMongoDbShellFilePath.Value;
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

        var mongoDbShellFilePath = await GetMongoDbShellFilePathAsync(ct)
            .ConfigureAwait(false);

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, fileMode: Unix.FileMode644, ct: ct)
            .ConfigureAwait(false);

        var command = new[]
        {
            mongoDbShellFilePath,
            "--username", _configuration.Username,
            "--password", _configuration.Password,
            "--quiet",
            "--eval",
            $"load('{scriptFilePath}')",
        };

        return await ExecAsync(command, ct)
            .ConfigureAwait(false);
    }

    private async Task<string> FindMongoDbShellFilePathAsync()
    {
        var findMongoDbShellFilePathExecResult = await ExecAsync(FindMongoDbShellFilePath)
            .ConfigureAwait(false);

        if (findMongoDbShellFilePathExecResult.ExitCode == 0)
        {
            return findMongoDbShellFilePathExecResult.Stdout.Trim();
        }

        throw new NotSupportedException("The mongosh or mongo binary could not be found.");
    }
}