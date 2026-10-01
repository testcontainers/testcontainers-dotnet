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
    /// <remarks>
    /// The deployment runs as a single-node replica set that advertises the container's
    /// hostname, which the test host cannot resolve. The connection string therefore
    /// sets <c>directConnection=true</c>.
    /// </remarks>
    /// <returns>The MongoDb Atlas Local connection string.</returns>
    public string GetConnectionString()
    {
        // The MongoDb documentation recommends to use percent-encoding for username and password: https://www.mongodb.com/docs/manual/reference/connection-string/.
        var endpoint = new UriBuilder("mongodb", Hostname, GetMappedPublicPort(MongoDbAtlasLocalBuilder.MongoDbAtlasLocalPort));
        endpoint.UserName = Uri.EscapeDataString(_configuration.Username ?? string.Empty);
        endpoint.Password = Uri.EscapeDataString(_configuration.Password ?? string.Empty);
        endpoint.Query = "directConnection=true";
        return endpoint.ToString();
    }

    /// <summary>
    /// Executes the JavaScript script in the MongoDb Atlas Local container.
    /// </summary>
    /// <param name="scriptContent">The content of the JavaScript script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the JavaScript script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.UTF8.GetBytes(scriptContent), scriptFilePath, fileMode: Unix.FileMode644, ct: ct)
            .ConfigureAwait(false);

        var command = new List<string>();
        command.Add(MongoDbShellFilePath);

        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
        {
            command.Add("--username");
            command.Add(_configuration.Username);
            command.Add("--password");
            command.Add(_configuration.Password);
        }

        command.Add("--quiet");
        command.Add("--eval");
        command.Add($"load('{scriptFilePath}')");

        return await ExecAsync(command, ct)
            .ConfigureAwait(false);
    }
}