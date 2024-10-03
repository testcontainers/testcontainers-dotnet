namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerComposeContainer" />
internal sealed class DockerComposeLocalContainer : DockerComposeContainer
{
    private readonly DockerComposeConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeLocalContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerComposeLocalContainer(DockerComposeConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken ct = default)
    {
        var workingDirectoryPath = Path.GetDirectoryName(_configuration.ComposeFilePath);

        return Cli.Wrap(_configuration.Entrypoint.First())
            .WithArguments(_configuration.Entrypoint.Skip(1).Concat(_configuration.Command))
            .WithWorkingDirectory(workingDirectoryPath)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync(ct);
    }

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken ct = default)
    {
        var workingDirectoryPath = Path.GetDirectoryName(_configuration.ComposeFilePath);

        return Cli.Wrap(_configuration.Entrypoint.First())
            .WithArguments(_configuration.Entrypoint.Skip(1).Concat(new[] { "down" }))
            .WithWorkingDirectory(workingDirectoryPath)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync(ct);
    }
}