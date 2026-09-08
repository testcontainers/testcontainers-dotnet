namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="ImageFromDockerfileBuilder" />
  /// <remarks>
  /// Builds the Docker image with BuildKit (<c>docker buildx build</c>) instead of
  /// the Docker Engine API, which uses the legacy builder. Dockerfile instructions
  /// and options that require BuildKit, such as here-documents,
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
  ///   _ = new BuildKitImageFromDockerfileBuilder()
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
    /// The Docker CLI image that is used if no image is set.
    /// </summary>
    private const string DefaultCliImage = "docker:29.7.2-cli";

    /// <summary>
    /// The pattern that a build secret id and an SSH agent id must match.
    /// </summary>
    /// <remarks>
    /// The id is part of the path of the file that carries the build secret inside
    /// the Docker CLI container, and part of the Docker CLI argument that
    /// references it.
    /// </remarks>
    private static readonly Regex IdRegex = new Regex("^[A-Za-z0-9][A-Za-z0-9_.-]*$", RegexOptions.None, TimeSpan.FromSeconds(1));

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitImageFromDockerfileBuilder" /> class.
    /// </summary>
    public BuildKitImageFromDockerfileBuilder()
      : this(new DockerImage(DefaultCliImage))
    {
    }

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
    /// An <see cref="IImage" /> instance that specifies the Docker image that runs
    /// the image build.
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

    /// <summary>
    /// Sets the platform to build the image for.
    /// </summary>
    /// <remarks>
    /// The build result is written to the image store of the Docker daemon, which
    /// takes a single platform only. Building an image for a platform other than
    /// the platform of the Docker host requires emulation, such as QEMU.
    /// </remarks>
    /// <param name="platform">The platform to build the image for e.g. <c>--platform "linux/arm64"</c>.</param>
    /// <returns>A configured instance of <see cref="BuildKitImageFromDockerfileBuilder" />.</returns>
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
    /// <param name="id">The build secret id e.g. <c>--secret "id=aws,src=$HOME/.aws/credentials"</c>.</param>
    /// <param name="value">The build secret value.</param>
    /// <returns>A configured instance of <see cref="BuildKitImageFromDockerfileBuilder" />.</returns>
    public BuildKitImageFromDockerfileBuilder WithSecret(string id, string value)
    {
      var secrets = new[] { new BuildSecret(id, value) };
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
    /// <param name="id">The build secret id e.g. <c>--secret "id=aws,src=$HOME/.aws/credentials"</c>.</param>
    /// <param name="source">The file on the test host that contains the build secret value.</param>
    /// <returns>A configured instance of <see cref="BuildKitImageFromDockerfileBuilder" />.</returns>
    public BuildKitImageFromDockerfileBuilder WithSecret(string id, FileInfo source)
    {
      var secrets = new[] { new BuildSecret(id, source) };
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(secrets: secrets));
    }

    /// <summary>
    /// Sets an SSH agent socket or private key.
    /// </summary>
    /// <remarks>
    /// The Dockerfile mounts the SSH agent with
    /// <c>RUN --mount=type=ssh,id=&lt;id&gt;</c>. Use the id <c>default</c> for a
    /// mount that does not name an id.
    ///
    /// Each path is bind-mounted read-only into the Docker CLI container, keeping
    /// the path it has on the test host. The Docker daemon resolves the mount
    /// source, so the paths must exist on the host that runs the Docker daemon.
    /// </remarks>
    /// <param name="id">The SSH agent id e.g. <c>--ssh "default=$SSH_AUTH_SOCK"</c>.</param>
    /// <param name="paths">A list of SSH agent socket or private key paths on the test host.</param>
    /// <returns>A configured instance of <see cref="BuildKitImageFromDockerfileBuilder" />.</returns>
    public BuildKitImageFromDockerfileBuilder WithSshAgent(string id, params string[] paths)
    {
      var sshAgents = new Dictionary<string, IEnumerable<string>> { { id, paths.Select(Path.GetFullPath).ToArray() } };
      return Merge(DockerResourceConfiguration, new BuildKitImageFromDockerfileConfiguration(sshAgents: sshAgents));
    }

    /// <inheritdoc />
    public override IFutureDockerImage Build()
    {
      Validate();
      return new BuildKitDockerImage(DockerResourceConfiguration);
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
        .ThrowIf(argument => argument.Value.Any(secret => !IsIdValid(secret.Id)), argument => new ArgumentException(string.Format(secretIdInvalid, argument.Value.First(secret => !IsIdValid(secret.Id)).Id), argument.Name));

      const string secretIdNotUnique = "The build secret id '{0}' is set more than once.";
      _ = Guard.Argument(DockerResourceConfiguration.Secrets, nameof(DockerResourceConfiguration.Secrets))
        .ThrowIf(argument => argument.Value.GroupBy(secret => secret.Id).Any(group => group.Count() > 1), argument => new ArgumentException(string.Format(secretIdNotUnique, argument.Value.GroupBy(secret => secret.Id).First(group => group.Count() > 1).Key), argument.Name));

      const string sshAgentIdInvalid = "The SSH agent id '{0}' must start with a letter or digit and can only contain letters, digits, dots, dashes, and underscores.";
      _ = Guard.Argument(DockerResourceConfiguration.SshAgents, nameof(DockerResourceConfiguration.SshAgents))
        .ThrowIf(argument => argument.Value.Keys.Any(id => !IsIdValid(id)), argument => new ArgumentException(string.Format(sshAgentIdInvalid, argument.Value.Keys.First(id => !IsIdValid(id))), argument.Name));
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
    /// Checks whether a build secret or SSH agent id is valid or not.
    /// </summary>
    /// <param name="id">The build secret or SSH agent id.</param>
    /// <returns>True if the id is valid; otherwise, false.</returns>
    private static bool IsIdValid(string id)
    {
      return !string.IsNullOrEmpty(id) && IdRegex.IsMatch(id);
    }
  }
}
