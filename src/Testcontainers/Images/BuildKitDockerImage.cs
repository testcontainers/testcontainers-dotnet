namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
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
  using Microsoft.Extensions.Logging.Abstractions;

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

          var buildCommand = GetBuildCommand();

          // The build arguments are not secrets by contract, but they are not
          // necessarily harmless either. Log them redacted, the same way the Docker
          // Engine API image builder does not log the image build parameters at all.
          Logger.BuildDockerImage(_configuration.Image, RedactBuildArguments(buildCommand));

          var execResult = await cliContainer.ExecAsync(buildCommand, ct)
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
      // The Docker CLI container is an implementation detail of the image build. It
      // keeps the default Resource Reaper session and the default logger, no matter
      // how the image is configured. Disabling the cleanup of the image keeps the
      // built image, it does not keep the container that built it, which carries the
      // build secrets. Not logging to the configured logger keeps the Docker CLI
      // command, which carries the build arguments, out of the log output.
      var cliBuilder = new ContainerBuilder()
        .WithImage(_configuration.CliImage)
        .WithDockerEndpoint(_configuration.DockerEndpointAuthConfig)
        .WithLogger(NullLogger.Instance)
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
      catch (DockerApiException e) when (IsDockerSocketMountFailure(e))
      {
        // Building an image with BuildKit runs the Docker CLI against the Docker socket
        // of the Docker daemon. A Docker daemon that does not listen on a Unix socket,
        // such as a Docker daemon that is reached over a Windows named pipe, cannot
        // provide one. Every other failure, such as a Docker CLI image that cannot be
        // pulled, propagates unchanged.
        throw new InvalidOperationException($"The Docker socket '{GetDockerSocketFilePath()}' cannot be mounted into the Docker CLI container. Building an image with BuildKit requires a Docker socket that the Docker daemon can resolve. Set TestcontainersSettings.DockerSocketOverride to the Docker socket path of the Docker daemon, or use ImageFromDockerfileBuilder, which builds the image through the Docker Engine API.", e);
      }
    }

    /// <summary>
    /// Checks whether the Docker daemon rejected the bind mount of the Docker socket
    /// or not.
    /// </summary>
    /// <remarks>
    /// A Docker daemon that cannot resolve the Docker socket responds with a mount
    /// error that names the Docker socket, e.g.
    /// <c>invalid mount config for type "bind": bind source path does not exist:
    /// /var/run/docker.sock</c>.
    /// </remarks>
    /// <param name="e">The exception that the Docker daemon responded with.</param>
    /// <returns>True if the Docker daemon rejected the bind mount of the Docker socket; otherwise, false.</returns>
    private bool IsDockerSocketMountFailure(DockerApiException e)
    {
      return e.Message != null
        && e.Message.IndexOf("mount", StringComparison.OrdinalIgnoreCase) > -1
        && e.Message.IndexOf(GetDockerSocketFilePath(), StringComparison.Ordinal) > -1;
    }

    /// <summary>
    /// Gets the path of the Docker socket on the host that runs the Docker daemon.
    /// </summary>
    /// <returns>The path of the Docker socket.</returns>
    private string GetDockerSocketFilePath()
    {
      return new UnixSocketMount(_configuration.DockerEndpointAuthConfig.Endpoint).Source;
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
    /// CLI provides an argument for are passed on, the remaining ones are logged.
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

      foreach (var parameterName in GetUnsupportedParameterNames(buildParameters))
      {
        Logger.ImageBuildParameterNotSupported(parameterName);
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

      if (buildParameters.NoCache.HasValue && buildParameters.NoCache.Value)
      {
        buildCommand.Add("--no-cache");
      }

      if (IsPullEnabled(buildParameters.Pull))
      {
        buildCommand.Add("--pull");
      }

      if (!string.IsNullOrEmpty(buildParameters.NetworkMode))
      {
        buildCommand.Add("--network");
        buildCommand.Add(buildParameters.NetworkMode);
      }

      if (buildParameters.ShmSize.HasValue)
      {
        buildCommand.Add("--shm-size");
        buildCommand.Add(buildParameters.ShmSize.Value.ToString(CultureInfo.InvariantCulture));
      }

      foreach (var extraHost in buildParameters.ExtraHosts ?? Array.Empty<string>())
      {
        buildCommand.Add("--add-host");
        buildCommand.Add(extraHost);
      }

      foreach (var cacheFrom in buildParameters.CacheFrom ?? Array.Empty<string>())
      {
        buildCommand.Add("--cache-from");
        buildCommand.Add(cacheFrom);
      }

      foreach (var tag in buildParameters.Tags ?? Array.Empty<string>())
      {
        buildCommand.Add("--tag");
        buildCommand.Add(tag);
      }

      foreach (var buildArgument in buildParameters.BuildArgs ?? Enumerable.Empty<KeyValuePair<string, string>>())
      {
        buildCommand.Add("--build-arg");
        buildCommand.Add($"{buildArgument.Key}={buildArgument.Value}");
      }

      foreach (var label in buildParameters.Labels ?? Enumerable.Empty<KeyValuePair<string, string>>())
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

    /// <summary>
    /// Checks whether the image build parameters pull the base images or not.
    /// </summary>
    /// <remarks>
    /// The Docker Engine API takes the option as a string, the Docker CLI as a flag.
    /// </remarks>
    /// <param name="pull">The value of the pull image build parameter.</param>
    /// <returns>True if the base images are pulled; otherwise, false.</returns>
    private static bool IsPullEnabled(string pull)
    {
      return !string.IsNullOrEmpty(pull)
        && !"0".Equals(pull, StringComparison.Ordinal)
        && !bool.FalseString.Equals(pull, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the names of the image build parameters that are set, but that the
    /// Docker CLI does not provide an equivalent argument for.
    /// </summary>
    /// <remarks>
    /// The resource limits of the legacy builder (CPU, memory) do not apply to a
    /// BuildKit build. The remaining parameters either configure the Docker Engine
    /// API image builder itself, or conflict with the way the Docker CLI container
    /// runs the image build.
    /// </remarks>
    /// <param name="buildParameters">The image build parameters.</param>
    /// <returns>The names of the image build parameters that are not applied.</returns>
    private static IEnumerable<string> GetUnsupportedParameterNames(ImageBuildParameters buildParameters)
    {
      if (buildParameters.SuppressOutput.HasValue)
      {
        yield return nameof(ImageBuildParameters.SuppressOutput);
      }

      if (!string.IsNullOrEmpty(buildParameters.RemoteContext))
      {
        yield return nameof(ImageBuildParameters.RemoteContext);
      }

      if (buildParameters.Remove.HasValue)
      {
        yield return nameof(ImageBuildParameters.Remove);
      }

      if (buildParameters.ForceRemove.HasValue)
      {
        yield return nameof(ImageBuildParameters.ForceRemove);
      }

      if (!string.IsNullOrEmpty(buildParameters.CPUSetCPUs))
      {
        yield return nameof(ImageBuildParameters.CPUSetCPUs);
      }

      if (buildParameters.CPUShares.HasValue)
      {
        yield return nameof(ImageBuildParameters.CPUShares);
      }

      if (buildParameters.CPUQuota.HasValue)
      {
        yield return nameof(ImageBuildParameters.CPUQuota);
      }

      if (buildParameters.CPUPeriod.HasValue)
      {
        yield return nameof(ImageBuildParameters.CPUPeriod);
      }

      if (buildParameters.Memory.HasValue)
      {
        yield return nameof(ImageBuildParameters.Memory);
      }

      if (buildParameters.MemorySwap.HasValue)
      {
        yield return nameof(ImageBuildParameters.MemorySwap);
      }

      if (buildParameters.Squash.HasValue)
      {
        yield return nameof(ImageBuildParameters.Squash);
      }

      if (!string.IsNullOrEmpty(buildParameters.Outputs))
      {
        yield return nameof(ImageBuildParameters.Outputs);
      }

      if (!string.IsNullOrEmpty(buildParameters.Version))
      {
        yield return nameof(ImageBuildParameters.Version);
      }

      if (buildParameters.AuthConfigs != null && buildParameters.AuthConfigs.Count > 0)
      {
        yield return nameof(ImageBuildParameters.AuthConfigs);
      }
    }

    /// <summary>
    /// Redacts the build argument values of the Docker CLI command that builds the
    /// Docker image.
    /// </summary>
    /// <param name="buildCommand">The Docker CLI command that builds the Docker image.</param>
    /// <returns>The Docker CLI command with the build argument values redacted.</returns>
    private static IEnumerable<string> RedactBuildArguments(IEnumerable<string> buildCommand)
    {
      const string redacted = "***";

      var isBuildArgument = false;

      foreach (var argument in buildCommand)
      {
        var separatorIndex = isBuildArgument ? argument.IndexOf('=') : -1;
        yield return separatorIndex > -1 ? string.Concat(argument.Substring(0, separatorIndex + 1), redacted) : argument;
        isBuildArgument = "--build-arg".Equals(argument, StringComparison.Ordinal);
      }
    }
  }
}
