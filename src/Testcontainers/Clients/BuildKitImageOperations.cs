namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Logging.Abstractions;

  /// <inheritdoc cref="IBuildKitImageOperations" />
  /// <remarks>
  /// Runs the Docker CLI inside a container and builds the Docker image with
  /// BuildKit (<c>docker buildx build</c>) instead of the Docker Engine API, which
  /// does not support it. The build context is copied into the container, and the
  /// Docker socket is mounted to interact with the Docker host. This does not
  /// require a Docker CLI installation on the test host.
  /// </remarks>
  internal sealed class BuildKitImageOperations : IBuildKitImageOperations
  {
    /// <summary>
    /// The directory inside the Docker CLI container that contains the build context.
    /// </summary>
    private const string ContextDirectoryPath = "/testcontainers/context";

    /// <summary>
    /// The tar archive inside the Docker CLI container that contains the build context.
    /// </summary>
    private const string ContextArchiveFilePath = "/testcontainers/context.tar";

    private static readonly string[] BuildCommand = { "docker", "buildx", "build", "--load", "--progress", "plain" };

    private readonly IDockerImageOperations _imageOperations;

    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitImageOperations" /> class.
    /// </summary>
    /// <param name="imageOperations">The Docker image operations.</param>
    /// <param name="logger">The logger.</param>
    public BuildKitImageOperations(IDockerImageOperations imageOperations, ILogger logger)
    {
      _imageOperations = imageOperations;
      _logger = logger;
    }

    /// <inheritdoc />
    /// <exception cref="ImageBuildFailedException">Thrown when the Docker image build fails.</exception>
    public async Task<string> BuildAsync(IBuildKitImageFromDockerfileConfiguration configuration, ImageBuildParameters buildParameters, ITarArchive dockerfileArchive, CancellationToken ct = default)
    {
      var image = configuration.Image;

      var imageExists = await _imageOperations.ExistsWithIdAsync(image.FullName, ct)
        .ConfigureAwait(false);

      if (imageExists && configuration.DeleteIfExists.HasValue && configuration.DeleteIfExists.Value)
      {
        await _imageOperations.DeleteAsync(image, ct)
          .ConfigureAwait(false);
      }

      var contextArchiveFilePath = await dockerfileArchive.Tar(ct)
        .ConfigureAwait(false);

      IContainer cliContainer = null;

      try
      {
        cliContainer = CreateCliContainer(configuration, contextArchiveFilePath);

        await cliContainer.StartAsync(ct)
          .ConfigureAwait(false);

        await CopySecretsAsync(configuration, cliContainer, ct)
          .ConfigureAwait(false);

        await ExtractContextAsync(cliContainer, ct)
          .ConfigureAwait(false);

        await RunBuildCommandAsync(configuration, buildParameters, cliContainer, ct)
          .ConfigureAwait(false);
      }
      finally
      {
        // The tar archive is copied into the Docker CLI container when it starts, and
        // the container carries the build secrets. Remove both, no matter whether the
        // image build succeeded or not. The tar archive is removed first, because the
        // Resource Reaper removes a container that is left behind, but no tar archive.
        if (File.Exists(contextArchiveFilePath))
        {
          File.Delete(contextArchiveFilePath);
        }

        if (cliContainer != null)
        {
          await cliContainer.DisposeAsync()
            .ConfigureAwait(false);
        }
      }

      _logger.DockerImageBuilt(image);
      return image.FullName;
    }

    /// <summary>
    /// Runs the Docker CLI command that builds the Docker image.
    /// </summary>
    /// <param name="configuration">The Dockerfile configuration.</param>
    /// <param name="buildParameters">The image build parameters.</param>
    /// <param name="cliContainer">The container that runs the Docker CLI.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker image has been built.</returns>
    /// <exception cref="ImageBuildFailedException">Thrown when the Docker image build fails.</exception>
    private async Task RunBuildCommandAsync(IBuildKitImageFromDockerfileConfiguration configuration, ImageBuildParameters buildParameters, IContainer cliContainer, CancellationToken ct = default)
    {
      var image = configuration.Image;

      var buildCommand = GetBuildCommand(configuration, buildParameters);

      // The build arguments are not secrets by contract, but they are not
      // necessarily harmless either. Log them redacted, the same way the Docker
      // Engine API image builder does not log the image build parameters at all.
      _logger.BuildDockerImage(image, RedactBuildArguments(buildCommand));

      var execResult = await cliContainer.ExecAsync(buildCommand, ct)
        .ConfigureAwait(false);

      // The Docker CLI writes the build output to stderr. Log it either way, it is
      // the only trace of the image build that the test host gets.
      _logger.DockerImageBuildOutput(image, execResult);

      if (!0L.Equals(execResult.ExitCode))
      {
        throw new ImageBuildFailedException(image, new[] { new JSONError { Message = execResult.Stderr } });
      }

      var imageHasBeenCreated = await _imageOperations.ExistsWithIdAsync(image.FullName, ct)
        .ConfigureAwait(false);

      if (!imageHasBeenCreated)
      {
        throw new ImageBuildFailedException(image, Array.Empty<JSONError>());
      }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The image build parameters carry the configuration that the Docker Engine API
    /// image builder gets too, including the parameter modifiers that
    /// <c>WithCreateParameterModifier</c> sets. The Dockerfile path refers to the
    /// build context inside the Docker CLI container, not to the test host.
    /// </remarks>
    public ImageBuildParameters GetBuildParameters(IBuildKitImageFromDockerfileConfiguration configuration)
    {
      var dockerfileFilePath = string.Join("/", ContextDirectoryPath, Unix.Instance.NormalizePath(configuration.Dockerfile));

      var buildParameters = new ImageBuildParameters
      {
        Dockerfile = dockerfileFilePath,
        Target = configuration.Target,
        Platform = configuration.Platform,
        Tags = new List<string> { configuration.Image.FullName },
        BuildArgs = configuration.BuildArguments.ToDictionary(item => item.Key, item => item.Value),
        Labels = configuration.Labels.ToDictionary(item => item.Key, item => item.Value),
      };

      if (configuration.ParameterModifiers != null)
      {
        foreach (var parameterModifier in configuration.ParameterModifiers)
        {
          parameterModifier(buildParameters);
        }
      }

      return buildParameters;
    }

    /// <summary>
    /// Gets the Docker CLI command that builds the Docker image.
    /// </summary>
    /// <remarks>
    /// Only the image build parameters that the Docker CLI provides an argument for
    /// are passed on, the remaining ones are logged.
    /// </remarks>
    /// <param name="configuration">The Dockerfile configuration.</param>
    /// <param name="buildParameters">The image build parameters.</param>
    /// <returns>The Docker CLI command that builds the Docker image.</returns>
    private IList<string> GetBuildCommand(IBuildKitImageFromDockerfileConfiguration configuration, ImageBuildParameters buildParameters)
    {
      foreach (var parameterName in GetUnsupportedParameterNames(buildParameters))
      {
        _logger.ImageBuildParameterNotSupported(parameterName);
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

      if (buildParameters.NoCache.HasValue && buildParameters.NoCache.Value)
      {
        buildCommand.Add("--no-cache");
      }

      if (IsPullEnabled(buildParameters.Pull))
      {
        buildCommand.Add("--pull");
      }

      foreach (var extraHost in buildParameters.ExtraHosts ?? Enumerable.Empty<string>())
      {
        buildCommand.Add("--add-host");
        buildCommand.Add(extraHost);
      }

      foreach (var cacheFrom in buildParameters.CacheFrom ?? Enumerable.Empty<string>())
      {
        buildCommand.Add("--cache-from");
        buildCommand.Add(cacheFrom);
      }

      foreach (var tag in buildParameters.Tags ?? Enumerable.Empty<string>())
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

      foreach (var secret in configuration.Secrets)
      {
        buildCommand.Add("--secret");
        buildCommand.Add($"id={secret.Key},src={secret.Value.Target}");
      }

      foreach (var ssh in configuration.Ssh)
      {
        buildCommand.Add("--ssh");
        buildCommand.Add($"{ssh.Key}={string.Join(",", ssh.Value)}");
      }

      buildCommand.Add(ContextDirectoryPath);

      return buildCommand;
    }

    /// <summary>
    /// Creates the container that runs the Docker CLI.
    /// </summary>
    /// <param name="configuration">The Dockerfile configuration.</param>
    /// <param name="contextArchiveFilePath">The tar archive on the test host that contains the build context.</param>
    /// <returns>The container that runs the Docker CLI.</returns>
    private static IContainer CreateCliContainer(IBuildKitImageFromDockerfileConfiguration configuration, string contextArchiveFilePath)
    {
      // The Docker CLI container is an implementation detail of the image build. It
      // keeps the default Resource Reaper session and the default logger, no matter
      // how the image is configured. Disabling the cleanup of the image keeps the
      // built image, it does not keep the container that built it, which carries the
      // build secrets. Not logging to the configured logger keeps the Docker CLI
      // command, which carries the build arguments, out of the log output.
      var cliBuilder = new ContainerBuilder()
        .WithImage(configuration.CliImage)
        .WithDockerEndpoint(configuration.DockerEndpointAuthConfig)
        .WithLogger(NullLogger.Instance)
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand("trap 'exit 0' TERM; sleep infinity & wait $!")
        .WithMount(new UnixSocketMount(configuration.DockerEndpointAuthConfig.Endpoint))
        .WithResourceMapping(FilePath.Of(contextArchiveFilePath), FilePath.Of(ContextArchiveFilePath));

      // Bind-mount the SSH agent sockets and private keys, keeping the path they have
      // on the test host. The Docker daemon resolves the mount source, which is the
      // same reason the Docker socket can be mounted.
      cliBuilder = configuration.Ssh.Values
        .SelectMany(paths => paths)
        .Distinct()
        .Aggregate(cliBuilder, (builder, path) => builder.WithBindMount(path, path, AccessMode.ReadOnly));

      return cliBuilder.Build();
    }

    /// <summary>
    /// Copies the build secrets into the Docker CLI container.
    /// </summary>
    /// <remarks>
    /// The build secrets are copied after the Docker CLI container has been started.
    /// They are not part of the container configuration, which keeps them out of the
    /// Docker resource that the Docker daemon reports.
    /// </remarks>
    /// <param name="configuration">The Dockerfile configuration.</param>
    /// <param name="cliContainer">The container that runs the Docker CLI.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the build secrets have been copied.</returns>
    private static async Task CopySecretsAsync(IBuildKitImageFromDockerfileConfiguration configuration, IContainer cliContainer, CancellationToken ct = default)
    {
      foreach (var secret in configuration.Secrets.Values)
      {
        var secretValue = await secret.GetAllBytesAsync(ct)
          .ConfigureAwait(false);

        await cliContainer.CopyAsync(secretValue, secret.Target, secret.UserId, secret.GroupId, secret.FileMode, ct)
          .ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Extracts the build context inside the Docker CLI container.
    /// </summary>
    /// <param name="cliContainer">The container that runs the Docker CLI.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the build context has been extracted.</returns>
    private static async Task ExtractContextAsync(IContainer cliContainer, CancellationToken ct = default)
    {
      var extractContextCommand = new[] { "/bin/sh", "-c", $"mkdir -p '{ContextDirectoryPath}' && tar -xf '{ContextArchiveFilePath}' -C '{ContextDirectoryPath}' && rm '{ContextArchiveFilePath}'" };

      _ = await cliContainer.ExecAsync(extractContextCommand, ct)
        .ThrowOnFailure()
        .ConfigureAwait(false);
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

    /// <summary>
    /// Gets the names of the image build parameters that are set, but that the
    /// Docker CLI does not provide an equivalent argument for.
    /// </summary>
    /// <remarks>
    /// The resource limits of the Docker Engine API image build (CPU, memory) do
    /// not apply to a BuildKit build. The remaining parameters either configure the
    /// Docker Engine API image builder itself, or conflict with the way the Docker
    /// CLI container runs the image build.
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
    /// Checks whether the image build parameters pull the base images or not.
    /// </summary>
    /// <remarks>
    /// The Docker Engine API takes the option as a string, the Docker CLI as a flag.
    /// </remarks>
    /// <param name="pull">The value of the pull image build parameter.</param>
    /// <returns>True if the base images are pulled, false otherwise.</returns>
    private static bool IsPullEnabled(string pull)
    {
      return !string.IsNullOrEmpty(pull)
        && !"0".Equals(pull, StringComparison.Ordinal)
        && !bool.FalseString.Equals(pull, StringComparison.OrdinalIgnoreCase);
    }
  }
}
