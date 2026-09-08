namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="IFutureDockerImage" />
  /// <remarks>
  /// Runs the Docker CLI inside a container and builds the Docker image with
  /// BuildKit (<c>docker buildx build</c>). The build context is copied into the
  /// container, and the Docker socket is mounted to interact with the Docker host.
  /// This does not require a Docker CLI installation on the test host.
  /// </remarks>
  [PublicAPI]
  internal sealed class BuildKitDockerImage : Resource, IFutureDockerImage
  {
    /// <summary>
    /// The directory inside the Docker CLI container that contains the build context.
    /// </summary>
    private const string ContextDirectoryPath = "/tmp/testcontainers/context";

    /// <summary>
    /// The tar archive inside the Docker CLI container that contains the build context.
    /// </summary>
    private const string ContextArchiveFilePath = "/tmp/testcontainers/context.tar";

    private static readonly string[] BuildCommand = { "docker", "buildx", "build", "--load", "--progress", "plain" };

    private readonly ITestcontainersClient _client;

    private readonly IBuildKitImageFromDockerfileConfiguration _configuration;

    private ImageInspectResponse _image = new ImageInspectResponse();

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitDockerImage" /> class.
    /// </summary>
    /// <param name="configuration">The image configuration.</param>
    public BuildKitDockerImage(IBuildKitImageFromDockerfileConfiguration configuration)
    {
      _client = new TestcontainersClient(configuration.SessionId, configuration.DockerEndpointAuthConfig, configuration.Logger);
      _configuration = configuration;
    }

    /// <inheritdoc />
    public string Repository
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.Repository;
      }
    }

    /// <inheritdoc />
    public string Registry
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.Registry;
      }
    }

    /// <inheritdoc />
    public string Tag
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.Tag;
      }
    }

    /// <inheritdoc />
    public string Digest
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.Digest;
      }
    }

    /// <inheritdoc />
    public string Platform
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.Platform;
      }
    }

    /// <inheritdoc />
    public string FullName
    {
      get
      {
        ThrowIfResourceNotFound();
        return _configuration.Image.FullName;
      }
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    private ILogger Logger
    {
      get
      {
        return _configuration.Logger;
      }
    }

    /// <inheritdoc />
    public string GetHostname()
    {
      ThrowIfResourceNotFound();
      return _configuration.Image.GetHostname();
    }

    /// <inheritdoc />
    public bool MatchLatestOrNightly()
    {
      return _configuration.Image.MatchLatestOrNightly();
    }

    /// <inheritdoc />
    public bool MatchVersion(Predicate<string> predicate)
    {
      return _configuration.Image.MatchVersion(predicate);
    }

    /// <inheritdoc />
    public bool MatchVersion(Predicate<System.Version> predicate)
    {
      return _configuration.Image.MatchVersion(predicate);
    }

    /// <inheritdoc />
    public async Task CreateAsync(CancellationToken ct = default)
    {
      using var disposable = await AcquireLockAsync(ct)
        .ConfigureAwait(false);

      await UnsafeCreateAsync(ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CancellationToken ct = default)
    {
      using var disposable = await AcquireLockAsync(ct)
        .ConfigureAwait(false);

      await UnsafeDeleteAsync(ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override bool Exists()
    {
      return !string.IsNullOrEmpty(_image.ID);
    }

    /// <inheritdoc />
    protected override async Task UnsafeCreateAsync(CancellationToken ct = default)
    {
      ThrowIfLockNotAcquired();

      if (Exists())
      {
        return;
      }

      await _client.System.LogContainerRuntimeInfoAsync(ct)
        .ConfigureAwait(false);

      ImageInspectResponse cachedImage;

      try
      {
        cachedImage = await _client.Image.ByIdAsync(_configuration.Image.FullName, ct)
          .ConfigureAwait(false);
      }
      catch (DockerImageNotFoundException)
      {
        cachedImage = null;
      }

      if (_configuration.ImageBuildPolicy(cachedImage))
      {
        await BuildAsync(ct)
          .ConfigureAwait(false);
      }

      _image = await _client.Image.ByIdAsync(_configuration.Image.FullName, ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
      ThrowIfLockNotAcquired();

      if (!Exists())
      {
        return;
      }

      await _client.Image.DeleteAsync(_configuration.Image, ct)
        .ConfigureAwait(false);

      _image = new ImageInspectResponse();
    }

    /// <summary>
    /// Builds the Docker image with BuildKit.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker image has been built.</returns>
    /// <exception cref="ImageBuildFailedException">The Docker image build failed.</exception>
    private async Task BuildAsync(CancellationToken ct = default)
    {
      var dockerfileArchive = new DockerfileArchive(
        _configuration.ContextDirectory,
        _configuration.DockerfileDirectory,
        _configuration.Dockerfile,
        _configuration.Image,
        _configuration.BuildArguments,
        Logger);

      // BuildKit resolves the base images itself, but it does not have access to the
      // Docker configuration of the test host. Pull them from the test host instead,
      // so that its Docker credentials and credential helpers apply.
      await PullBaseImagesAsync(dockerfileArchive, ct)
        .ConfigureAwait(false);

      var imageExists = await _client.Image.ExistsWithIdAsync(_configuration.Image.FullName, ct)
        .ConfigureAwait(false);

      if (imageExists && _configuration.DeleteIfExists.HasValue && _configuration.DeleteIfExists.Value)
      {
        await _client.Image.DeleteAsync(_configuration.Image, ct)
          .ConfigureAwait(false);
      }

      var contextArchiveFilePath = await dockerfileArchive.Tar(ct)
        .ConfigureAwait(false);

      try
      {
        var cliContainer = CreateCliContainer(contextArchiveFilePath);

        try
        {
          await StartCliContainerAsync(cliContainer, ct)
            .ConfigureAwait(false);

          // Copy the build secrets after the Docker CLI container has been started. They
          // are not part of the container configuration, which keeps them out of the
          // Docker resource that the Docker daemon reports.
          foreach (var secret in _configuration.Secrets)
          {
            var secretValue = await secret.GetAllBytesAsync(ct)
              .ConfigureAwait(false);

            await cliContainer.CopyAsync(secretValue, secret.FilePath, 0, 0, secret.FileMode, ct)
              .ConfigureAwait(false);
          }

          _ = await cliContainer.ExecAsync(GetExtractContextCommand(), ct)
            .ThrowOnFailure()
            .ConfigureAwait(false);

          var execResult = await cliContainer.ExecAsync(GetBuildCommand(), ct)
            .ConfigureAwait(false);

          // The Docker CLI writes the build output to stderr. Log it either way, it is
          // the only trace of the image build that the test host gets.
          Logger.DockerImageBuildOutput(_configuration.Image, string.Concat(execResult.Stdout, execResult.Stderr));

          if (!0L.Equals(execResult.ExitCode))
          {
            throw new ImageBuildFailedException(_configuration.Image, new[] { new JSONError { Message = execResult.Stderr } });
          }

          var imageHasBeenCreated = await _client.Image.ExistsWithIdAsync(_configuration.Image.FullName, ct)
            .ConfigureAwait(false);

          if (!imageHasBeenCreated)
          {
            throw new ImageBuildFailedException(_configuration.Image, Array.Empty<JSONError>());
          }

          Logger.DockerImageBuilt(_configuration.Image);
        }
        finally
        {
          await cliContainer.DisposeAsync()
            .ConfigureAwait(false);
        }
      }
      finally
      {
        File.Delete(contextArchiveFilePath);
      }
    }

    /// <summary>
    /// Pulls the base images of the Dockerfile that are not present on the Docker
    /// host.
    /// </summary>
    /// <param name="dockerfileArchive">The Dockerfile archive that resolves the base images.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the base images have been pulled.</returns>
    private async Task PullBaseImagesAsync(DockerfileArchive dockerfileArchive, CancellationToken ct = default)
    {
      var baseImages = dockerfileArchive.GetBaseImages().ToArray();

      var filters = baseImages.Aggregate(new FilterByProperty(), (dictionary, baseImage) => dictionary.Add("reference", baseImage.FullName));

      var cachedImages = await _client.Image.GetAllAsync(filters, ct)
        .ConfigureAwait(false);

      var repositoryTags = new HashSet<string>(cachedImages.SelectMany(image => image.RepoTags ?? Array.Empty<string>()));

      var uncachedImages = baseImages.Where(baseImage => !repositoryTags.Contains(baseImage.FullName));

      await Task.WhenAll(uncachedImages.Select(image => _client.PullImageAsync(image, ct)))
        .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates the container that runs the Docker CLI.
    /// </summary>
    /// <param name="contextArchiveFilePath">The tar archive on the test host that contains the build context.</param>
    /// <returns>The container that runs the Docker CLI.</returns>
    private IContainer CreateCliContainer(string contextArchiveFilePath)
    {
      // The Docker CLI container shares the Resource Reaper session of the image.
      // The session is not necessarily the default one, disabling the cleanup sets an
      // empty session id that the Resource Reaper ignores.
      var cliBuilder = new ContainerBuilder()
        .WithImage(_configuration.CliImage)
        .WithDockerEndpoint(_configuration.DockerEndpointAuthConfig)
        .WithLabel(ResourceReaper.ResourceReaperSessionLabel, _configuration.SessionId.ToString("D"))
        .WithLogger(Logger)
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand("trap 'exit 0' TERM; sleep infinity & wait $!")
        .WithMount(new UnixSocketMount(_configuration.DockerEndpointAuthConfig.Endpoint))
        .WithResourceMapping(new FileInfo(contextArchiveFilePath), new FileInfo(ContextArchiveFilePath));

      // Bind-mount the SSH agent sockets and private keys, keeping the path they have
      // on the test host. The Docker daemon resolves the mount source, which is the
      // same reason the Docker socket can be mounted.
      cliBuilder = _configuration.SshAgents.Values
        .SelectMany(paths => paths)
        .Distinct()
        .Aggregate(cliBuilder, (builder, path) => builder.WithBindMount(path, path, AccessMode.ReadOnly));

      return cliBuilder.Build();
    }

    /// <summary>
    /// Starts the container that runs the Docker CLI.
    /// </summary>
    /// <param name="cliContainer">The container that runs the Docker CLI.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been started.</returns>
    /// <exception cref="InvalidOperationException">The Docker socket cannot be mounted into the container.</exception>
    private async Task StartCliContainerAsync(IContainer cliContainer, CancellationToken ct = default)
    {
      try
      {
        await cliContainer.StartAsync(ct)
          .ConfigureAwait(false);
      }
      catch (DockerApiException e)
      {
        // Building an image with BuildKit runs the Docker CLI against the Docker socket
        // of the Docker daemon. A Docker daemon that does not listen on a Unix socket,
        // such as a Docker daemon that is reached over a Windows named pipe, cannot
        // provide one.
        var dockerSocketFilePath = new UnixSocketMount(_configuration.DockerEndpointAuthConfig.Endpoint).Source;
        throw new InvalidOperationException($"The Docker socket '{dockerSocketFilePath}' cannot be mounted into the Docker CLI container. Building an image with BuildKit requires a Docker socket that the Docker daemon can resolve. Set TestcontainersSettings.DockerSocketOverride to the Docker socket path of the Docker daemon, or use ImageFromDockerfileBuilder, which builds the image through the Docker Engine API.", e);
      }
    }

    /// <summary>
    /// Gets the command that extracts the build context inside the Docker CLI
    /// container.
    /// </summary>
    /// <returns>The command that extracts the build context.</returns>
    private static IList<string> GetExtractContextCommand()
    {
      return new[] { "/bin/sh", "-c", $"mkdir -p '{ContextDirectoryPath}' && tar -xf '{ContextArchiveFilePath}' -C '{ContextDirectoryPath}' && rm '{ContextArchiveFilePath}'" };
    }

    /// <summary>
    /// Gets the Docker CLI command that builds the Docker image.
    /// </summary>
    /// <remarks>
    /// The image build parameters carry the configuration that the Docker Engine API
    /// image builder gets too, including the parameter modifiers that
    /// <c>WithCreateParameterModifier</c> sets. Only the parameters that the Docker
    /// CLI supports are passed on.
    /// </remarks>
    /// <returns>The Docker CLI command that builds the Docker image.</returns>
    private IList<string> GetBuildCommand()
    {
      var dockerfileFilePath = string.Join("/", ContextDirectoryPath, Unix.Instance.NormalizePath(_configuration.Dockerfile));

      var buildParameters = new ImageBuildParameters
      {
        Dockerfile = dockerfileFilePath,
        Target = _configuration.Target,
        Platform = _configuration.Platform,
        Tags = new List<string> { _configuration.Image.FullName },
        BuildArgs = _configuration.BuildArguments.ToDictionary(item => item.Key, item => item.Value),
        Labels = _configuration.Labels.ToDictionary(item => item.Key, item => item.Value),
      };

      if (_configuration.ParameterModifiers != null)
      {
        foreach (var parameterModifier in _configuration.ParameterModifiers)
        {
          parameterModifier(buildParameters);
        }
      }

      var buildCommand = new List<string>(BuildCommand);

      buildCommand.Add("--file");
      buildCommand.Add(buildParameters.Dockerfile);

      if (!string.IsNullOrEmpty(buildParameters.Target))
      {
        buildCommand.Add("--target");
        buildCommand.Add(buildParameters.Target);
      }

      if (!string.IsNullOrEmpty(buildParameters.Platform))
      {
        buildCommand.Add("--platform");
        buildCommand.Add(buildParameters.Platform);
      }

      foreach (var tag in buildParameters.Tags)
      {
        buildCommand.Add("--tag");
        buildCommand.Add(tag);
      }

      foreach (var buildArgument in buildParameters.BuildArgs)
      {
        buildCommand.Add("--build-arg");
        buildCommand.Add($"{buildArgument.Key}={buildArgument.Value}");
      }

      foreach (var label in buildParameters.Labels)
      {
        buildCommand.Add("--label");
        buildCommand.Add($"{label.Key}={label.Value}");
      }

      foreach (var secret in _configuration.Secrets)
      {
        buildCommand.Add("--secret");
        buildCommand.Add($"id={secret.Id},src={secret.FilePath}");
      }

      foreach (var sshAgent in _configuration.SshAgents)
      {
        buildCommand.Add("--ssh");
        buildCommand.Add(sshAgent.Value.Any() ? $"{sshAgent.Key}={string.Join(",", sshAgent.Value)}" : sshAgent.Key);
      }

      buildCommand.Add(ContextDirectoryPath);

      return buildCommand;
    }
  }
}
