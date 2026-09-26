namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="ImageFromDockerfileBuilder" />
  /// <remarks>
  /// Builds the Docker image with BuildKit (<c>docker buildx build</c>) instead of
  /// the Docker Engine API, which does not support it. Dockerfile instructions and
  /// options that require BuildKit, such as here-documents,
  /// <c>RUN --mount=type=secret</c> and <c># syntax=</c> frontends, are only
  /// available with this builder.
  ///
  /// The Docker CLI runs inside a container. The build context is copied into the
  /// container, and the Docker socket is mounted to interact with the Docker host.
  /// This does not require a Docker CLI installation on the test host. The build
  /// itself runs in the BuildKit instance of the Docker daemon (the default
  /// <c>docker</c> buildx driver), which is also where the build cache is kept.
  /// The built image is written to the image store of the Docker daemon
  /// (<c>--load</c>), so it can be used like any other image.
  /// </remarks>
  /// <example>
  ///   The default configuration is equivalent to:
  ///   <code>
  ///   _ = new BuildKitImageFromDockerfileBuilder(cliImage)
  ///     .WithDockerEndpoint(TestcontainersSettings.OS.DockerEndpointAuthConfig)
  ///     .WithLabel(DefaultLabels.Instance)
  ///     .WithCleanUp(true)
  ///     .WithImageBuildPolicy(PullPolicy.Always)
  ///     .WithDockerfile("Dockerfile")
  ///     .WithDockerfileDirectory(Directory.GetCurrentDirectory())
  ///     .WithName(new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty))
  ///     .Build();
  ///   </code>
  /// </example>
  [PublicAPI]
  public sealed class BuildKitImageFromDockerfileBuilder : AbstractBuilder<BuildKitImageFromDockerfileBuilder, IFutureDockerImage, ImageBuildParameters, IBuildKitImageFromDockerfileConfiguration>, IImageFromDockerfileBuilder<BuildKitImageFromDockerfileBuilder>
  {
    /// <summary>
    /// The pattern that a build secret id and an SSH id must match.
    /// </summary>
    /// <remarks>
    /// The id is part of the path of the file that carries the build secret inside the
    /// Docker CLI container, and part of the Docker CLI argument that references it.
    /// </remarks>
    private static readonly Regex IdRegex = new Regex("^[A-Za-z0-9][A-Za-z0-9_.-]*$", RegexOptions.None, TimeSpan.FromSeconds(1));

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitImageFromDockerfileBuilder" /> class.
    /// </summary>
    /// <param name="cliImage">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>docker:29-cli</c>).
    /// </param>
    /// <remarks>
    /// The image requires the Docker Buildx plugin. Docker image tags available at
    /// <see href="https://hub.docker.com/_/docker/tags" />.
    /// </remarks>
    public BuildKitImageFromDockerfileBuilder(string cliImage)
      : this(new DockerImage(cliImage))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitImageFromDockerfileBuilder" /> class.
    /// </summary>
    /// <param name="cliImage">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the image builder configuration.
    /// </param>
    /// <remarks>
    /// The image requires the Docker Buildx plugin. Docker image tags available at
    /// <see href="https://hub.docker.com/_/docker/tags" />.
    /// </remarks>
    public BuildKitImageFromDockerfileBuilder(IImage cliImage)
      : this(new BuildKitImageFromDockerfileConfiguration())
    {
      DockerResourceConfiguration = Init().WithCliImage(cliImage).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitImageFromDockerfileBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private BuildKitImageFromDockerfileBuilder(IBuildKitImageFromDockerfileConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
      DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override IBuildKitImageFromDockerfileConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithName(string name)
    {
      return WithName(new DockerImage(name));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithName(IImage image)
    {
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(image: image.ApplyImageNameSubstitution()));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithContextDirectory(string contextDirectory)
    {
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(contextDirectory: contextDirectory));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithDockerfile(string dockerfile)
    {
      var dockerfileFilePath = Regex.Replace(dockerfile, "^\\.(\\/|\\\\)", string.Empty, RegexOptions.None, TimeSpan.FromSeconds(1));
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(dockerfile: dockerfileFilePath));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(dockerfileDirectory: dockerfileDirectory));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithDockerfileDirectory(CommonDirectoryPath commonDirectoryPath, string dockerfileDirectory)
    {
      var dockerfileDirectoryPath = Path.Combine(commonDirectoryPath.DirectoryPath, dockerfileDirectory);
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(dockerfileDirectory: dockerfileDirectoryPath));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithTarget(string target)
    {
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(target: target));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithImageBuildPolicy(Func<ImageInspectResponse, bool> imageBuildPolicy)
    {
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(imageBuildPolicy: imageBuildPolicy));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(deleteIfExists: deleteIfExists));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithBuildArgument(string name, string value)
    {
      var buildArguments = new Dictionary<string, string> { { name, value } };
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(buildArguments: buildArguments));
    }

    /// <inheritdoc />
    public BuildKitImageFromDockerfileBuilder WithPlatform(string platform)
    {
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(platform: platform));
    }

    /// <summary>
    /// Sets a build secret.
    /// </summary>
    /// <remarks>
    /// The Dockerfile mounts the build secret with
    /// <c>RUN --mount=type=secret,id=&lt;id&gt;</c>, which makes it available at
    /// <c>/run/secrets/&lt;id&gt;</c> for the duration of that instruction only.
    /// BuildKit does not add the build secret to a layer of the built image.
    ///
    /// The build secret value is copied into the Docker CLI container that runs the
    /// image build. It is not part of the build context, and is not passed as a
    /// build argument or an environment variable.
    /// </remarks>
    /// <param name="id">The build secret id (e.g., <c>--secret "id=aws,src=$HOME/.aws/credentials"</c>).</param>
    /// <param name="value">The build secret value.</param>
    /// <returns>A configured instance of <see cref="BuildKitImageFromDockerfileBuilder" />.</returns>
    public BuildKitImageFromDockerfileBuilder WithSecret(string id, string value)
    {
      var secrets = new Dictionary<string, IResourceMapping> { { id, new BinaryResourceMapping(Encoding.Default.GetBytes(value), GetSecretFilePath(id), 0, 0, Unix.FileMode600) } };
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(secrets: secrets));
    }

    /// <summary>
    /// Sets a build secret.
    /// </summary>
    /// <remarks>
    /// The Dockerfile mounts the build secret with
    /// <c>RUN --mount=type=secret,id=&lt;id&gt;</c>, which makes it available at
    /// <c>/run/secrets/&lt;id&gt;</c> for the duration of that instruction only.
    /// BuildKit does not add the build secret to a layer of the built image.
    ///
    /// The build secret value is copied into the Docker CLI container that runs the
    /// image build. It is not part of the build context, and is not passed as a
    /// build argument or an environment variable.
    /// </remarks>
    /// <param name="id">The build secret id (e.g., <c>--secret "id=aws,src=$HOME/.aws/credentials"</c>).</param>
    /// <param name="source">The file on the test host that contains the build secret value.</param>
    /// <returns>A configured instance of <see cref="BuildKitImageFromDockerfileBuilder" />.</returns>
    public BuildKitImageFromDockerfileBuilder WithSecret(string id, FileInfo source)
    {
      var secrets = new Dictionary<string, IResourceMapping> { { id, new FileResourceMapping(source.FullName, GetSecretFilePath(id), 0, 0, Unix.FileMode600) } };
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(secrets: secrets));
    }

    /// <summary>
    /// Sets an SSH agent socket or private key to expose to the build.
    /// </summary>
    /// <remarks>
    /// The Dockerfile mounts it with <c>RUN --mount=type=ssh,id=&lt;id&gt;</c>. Use
    /// the id <c>default</c> for a mount that does not name an id.
    ///
    /// Each path is bind-mounted read-only into the Docker CLI container, keeping
    /// the path it has on the test host. The Docker daemon resolves the mount
    /// source, so the paths must exist on the host that runs the Docker daemon.
    /// At least one path is required, because the Docker CLI container does not
    /// run an SSH agent that an id without a path could resolve to.
    /// </remarks>
    /// <param name="id">The SSH id (e.g., <c>--ssh "default=$SSH_AUTH_SOCK"</c>).</param>
    /// <param name="paths">A list of SSH agent socket or private key paths on the test host.</param>
    /// <returns>A configured instance of <see cref="BuildKitImageFromDockerfileBuilder" />.</returns>
    public BuildKitImageFromDockerfileBuilder WithSsh(string id, params string[] paths)
    {
      // An unset path, such as an unset SSH_AUTH_SOCK environment variable, is kept
      // as it is. It cannot be resolved, and Validate reports it.
      var ssh = new Dictionary<string, IEnumerable<string>> { { id, paths.Select(path => string.IsNullOrEmpty(path) ? path : Path.GetFullPath(path)).ToArray() } };
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(ssh: ssh));
    }

    /// <inheritdoc />
    public override IFutureDockerImage Build()
    {
      Validate();
      return new FutureDockerImage(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override BuildKitImageFromDockerfileBuilder Init()
    {
      return base.Init().WithImageBuildPolicy(PullPolicy.Always).WithDockerfile("Dockerfile").WithDockerfileDirectory(Directory.GetCurrentDirectory()).WithName(new DockerImage(string.Join("/", "localhost", "testcontainers", Guid.NewGuid().ToString("D"))));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
      base.Validate();

      const string reuseNotSupported = "Building an image does not support the reuse feature. To keep the built image, disable the cleanup.";
      _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration.Reuse))
        .ThrowIf(argument => argument.Value.Reuse.HasValue && argument.Value.Reuse.Value, argument => new ArgumentException(reuseNotSupported, argument.Name));

      _ = Guard.Argument(DockerResourceConfiguration.CliImage, nameof(DockerResourceConfiguration.CliImage))
        .NotNull();

      const string secretIdInvalid = "The build secret id '{0}' must start with a letter or digit and can only contain letters, digits, dots, dashes, and underscores.";
      _ = Guard.Argument(DockerResourceConfiguration.Secrets, nameof(DockerResourceConfiguration.Secrets))
        .ThrowIf(argument => argument.Value.Keys.Any(id => !IsIdValid(id)), argument => new ArgumentException(string.Format(secretIdInvalid, argument.Value.Keys.First(id => !IsIdValid(id))), argument.Name));

      const string secretSourceDoesNotExist = "The build secret file '{0}' does not exist.";
      _ = Guard.Argument(DockerResourceConfiguration.Secrets, nameof(DockerResourceConfiguration.Secrets))
        .ThrowIf(argument => argument.Value.Values.Any(IsSecretSourceMissing), argument => new FileNotFoundException(string.Format(secretSourceDoesNotExist, argument.Value.Values.First(IsSecretSourceMissing).Source)));

      const string sshIdInvalid = "The SSH id '{0}' must start with a letter or digit and can only contain letters, digits, dots, dashes, and underscores.";
      _ = Guard.Argument(DockerResourceConfiguration.Ssh, nameof(DockerResourceConfiguration.Ssh))
        .ThrowIf(argument => argument.Value.Keys.Any(id => !IsIdValid(id)), argument => new ArgumentException(string.Format(sshIdInvalid, argument.Value.Keys.First(id => !IsIdValid(id))), argument.Name));

      const string sshPathNotSet = "The SSH id '{0}' does not set a path. The Docker CLI container does not run an SSH agent, so at least one SSH agent socket or private key path is required.";
      _ = Guard.Argument(DockerResourceConfiguration.Ssh, nameof(DockerResourceConfiguration.Ssh))
        .ThrowIf(argument => argument.Value.Any(IsSshPathMissing), argument => new ArgumentException(string.Format(sshPathNotSet, argument.Value.First(IsSshPathMissing).Key), argument.Name));

      const string sshPathInvalid = "The SSH path '{0}' cannot contain a comma, which separates the paths of an SSH id.";
      _ = Guard.Argument(DockerResourceConfiguration.Ssh, nameof(DockerResourceConfiguration.Ssh))
        .ThrowIf(argument => GetSshPaths(argument.Value).Any(IsPathInvalid), argument => new ArgumentException(string.Format(sshPathInvalid, GetSshPaths(argument.Value).First(IsPathInvalid)), argument.Name));
    }

    /// <inheritdoc />
    protected override BuildKitImageFromDockerfileBuilder Clone(IResourceConfiguration<ImageBuildParameters> resourceConfiguration)
    {
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override BuildKitImageFromDockerfileBuilder Merge(IBuildKitImageFromDockerfileConfiguration oldValue, IBuildKitImageFromDockerfileConfiguration newValue)
    {
      return new BuildKitImageFromDockerfileBuilder(new BuildKitImageFromDockerfileConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Sets the Docker CLI image that runs the image build.
    /// </summary>
    /// <param name="cliImage">The Docker CLI image.</param>
    /// <returns>A configured instance of <see cref="BuildKitImageFromDockerfileBuilder" />.</returns>
    private BuildKitImageFromDockerfileBuilder WithCliImage(IImage cliImage)
    {
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(cliImage: cliImage));
    }

    /// <summary>
    /// Gets the path of the file inside the Docker CLI container that contains the
    /// build secret value.
    /// </summary>
    /// <remarks>
    /// The build secret value is copied into the Docker CLI container that runs the
    /// image build, not into the build context. It never becomes part of the build
    /// context tar archive or of a layer of the built image.
    /// </remarks>
    /// <param name="id">The build secret id.</param>
    /// <returns>The path of the file inside the Docker CLI container.</returns>
    private static string GetSecretFilePath(string id)
    {
      return string.Join("/", string.Empty, "tmp", "testcontainers", "secrets", id);
    }

    /// <summary>
    /// Gets the SSH agent socket and private key paths of all SSH ids.
    /// </summary>
    /// <param name="ssh">A dictionary of SSH agent sockets or private keys.</param>
    /// <returns>The SSH agent socket and private key paths.</returns>
    private static IEnumerable<string> GetSshPaths(IReadOnlyDictionary<string, IEnumerable<string>> ssh)
    {
      return ssh.Values.SelectMany(paths => paths);
    }

    /// <summary>
    /// Checks whether a build secret or SSH id is valid or not.
    /// </summary>
    /// <param name="id">The build secret or SSH id.</param>
    /// <returns>True if the id is valid; otherwise, false.</returns>
    private static bool IsIdValid(string id)
    {
      return !string.IsNullOrEmpty(id) && IdRegex.IsMatch(id);
    }

    /// <summary>
    /// Checks whether the file that contains the build secret value is missing or not.
    /// </summary>
    /// <param name="secret">The resource mapping that provides the build secret value.</param>
    /// <returns>True if the build secret reads its value from a file that does not exist; otherwise, false.</returns>
    private static bool IsSecretSourceMissing(IResourceMapping secret)
    {
      return !string.IsNullOrEmpty(secret.Source) && !File.Exists(secret.Source);
    }

    /// <summary>
    /// Checks whether an SSH id is missing its socket and private key paths or not.
    /// </summary>
    /// <remarks>
    /// The Docker CLI resolves an SSH id that does not name a path against
    /// <c>SSH_AUTH_SOCK</c>, which the Docker CLI container does not set.
    /// </remarks>
    /// <param name="ssh">The SSH id and its socket and private key paths.</param>
    /// <returns>True if the SSH id does not set a path or sets an empty path; otherwise, false.</returns>
    private static bool IsSshPathMissing(KeyValuePair<string, IEnumerable<string>> ssh)
    {
      return !ssh.Value.Any() || ssh.Value.Any(string.IsNullOrEmpty);
    }

    /// <summary>
    /// Checks whether an SSH agent socket or private key path is invalid or not.
    /// </summary>
    /// <remarks>
    /// The Docker CLI takes the paths of an SSH id as a comma-separated list. A path
    /// that contains a comma cannot be encoded.
    /// </remarks>
    /// <param name="path">The SSH agent socket or private key path.</param>
    /// <returns>True if the path is invalid; otherwise, false.</returns>
    private static bool IsPathInvalid(string path)
    {
      return path.IndexOf(',') > -1;
    }
  }
}
