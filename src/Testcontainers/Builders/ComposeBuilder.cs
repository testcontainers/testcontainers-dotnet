namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
  /// <remarks>
  /// Runs the Docker Compose CLI inside a container. The Docker Compose files are
  /// copied into the container, and the Docker socket is mounted to interact with
  /// the Docker host. This does not require a Docker Compose installation on the
  /// test host.
  ///
  /// Not only the Docker Compose files are copied, but every file in their
  /// directories (see <see cref="WithComposeFile(string[])" />). They keep the
  /// path they have on the test host, which allows Docker Compose to resolve
  /// relative bind mounts to a path that exists on the test host.
  /// </remarks>
  [PublicAPI]
  public sealed class ComposeBuilder : ContainerBuilder<ComposeBuilder, ComposeContainer, ComposeConfiguration>
  {
    /// <summary>
    /// The Docker Compose project name prefix that is used if no prefix is set.
    /// </summary>
    private const string DefaultProjectNamePrefix = "testcontainers-compose";

    /// <summary>
    /// The number of random characters that the Docker Compose project name ends with.
    /// </summary>
    private const ushort ProjectNameSuffixLength = 8;

    /// <summary>
    /// The maximum length of the Docker Compose project name. Docker Compose prefixes
    /// the container names with it, and a DNS label is limited to 63 characters.
    /// </summary>
    private const ushort ProjectNameMaxLength = 63;

    /// <summary>
    /// The maximum length of the Docker Compose project name prefix, that is, the
    /// project name without its random suffix and the dash that separates them.
    /// </summary>
    private const ushort ProjectNamePrefixMaxLength = ProjectNameMaxLength - ProjectNameSuffixLength - 1;

    /// <summary>
    /// The path separator characters that separate the Docker Compose file paths in
    /// <c>COMPOSE_FILE</c>, in the order they are considered.
    /// </summary>
    /// <remarks>
    /// A Unix path may contain the default path separator (colon). The path
    /// separator that Docker Compose uses is configurable, so that a path that
    /// contains one of them can still be passed (see
    /// <see cref="GetPathSeparator" />).
    /// </remarks>
    private static readonly string[] PathSeparators = { ":", ";", "|", "," };

    private static readonly Regex ProjectNameRegex = new Regex("^[a-z0-9][a-z0-9_-]*$", RegexOptions.None, TimeSpan.FromSeconds(1));

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>docker:28-cli</c>).
    /// </param>
    /// <remarks>
    /// The image requires the Docker Compose plugin. Docker image tags available at
    /// <see href="https://hub.docker.com/_/docker/tags" />.
    /// </remarks>
    public ComposeBuilder(string image)
      : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// The image requires the Docker Compose plugin. Docker image tags available at
    /// <see href="https://hub.docker.com/_/docker/tags" />.
    /// </remarks>
    public ComposeBuilder(IImage image)
      : this(new ComposeConfiguration())
    {
      DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ComposeBuilder(ComposeConfiguration resourceConfiguration)
      : base(resourceConfiguration)
    {
      DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ComposeConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Docker Compose files.
    /// </summary>
    /// <remarks>
    /// The directory of each Docker Compose file is copied into the container
    /// recursively, not just the Docker Compose file itself. Docker Compose
    /// resolves relative references, such as build contexts, environment files and
    /// included Docker Compose files, against that directory.
    ///
    /// Keep the Docker Compose files in a directory that contains only the files
    /// they reference. Placing them next to build output or other large directories
    /// copies those too, and slows down the start of the Docker Compose services.
    /// Use <see cref="WithCopyFilesInContainer(string[])" /> to copy the referenced
    /// files only.
    /// </remarks>
    /// <param name="composeFiles">A list of Docker Compose file paths.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithComposeFile(params string[] composeFiles)
    {
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(composeFiles: composeFiles));
    }

    /// <summary>
    /// Sets the files and directories to copy into the container.
    /// </summary>
    /// <remarks>
    /// By default, the entire directory of each Docker Compose file is copied into
    /// the container. Set the inclusions to copy the Docker Compose files and the
    /// listed files and directories only. Use it when the Docker Compose files sit
    /// next to files they do not reference, such as build output.
    ///
    /// The inclusions are resolved relative to the directory of the first Docker
    /// Compose file, and keep their relative path inside the container.
    /// </remarks>
    /// <param name="fileCopyInclusions">A list of file and directory paths, relative to the directory of the first Docker Compose file.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithCopyFilesInContainer(params string[] fileCopyInclusions)
    {
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(fileCopyInclusions: fileCopyInclusions));
    }

    /// <summary>
    /// Sets the Docker Compose project name prefix.
    /// </summary>
    /// <remarks>
    /// The project name is the prefix followed by a random suffix. A unique project
    /// name keeps concurrent test runs apart, and prevents the Resource Reaper from
    /// removing the Docker resources of a Docker Compose project that it does not
    /// own.
    /// </remarks>
    /// <param name="projectNamePrefix">The Docker Compose project name prefix.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithProjectNamePrefix(string projectNamePrefix)
    {
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(projectNamePrefix: projectNamePrefix));
    }

    /// <summary>
    /// Sets the Docker Compose services to start.
    /// </summary>
    /// <remarks>
    /// If no services are set, all services are started.
    /// </remarks>
    /// <param name="services">A list of Docker Compose service names.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithService(params string[] services)
    {
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(services: services));
    }

    /// <summary>
    /// Sets the number of containers that a Docker Compose service runs.
    /// </summary>
    /// <remarks>
    /// Each container is one instance of the service, addressed by the service name
    /// and the container number. Use
    /// <see cref="WithExposedServiceInstance(string, ushort, ushort)" /> to expose
    /// the port of a specific instance.
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="count">The number of containers that the Docker Compose service runs.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithScaledService(string serviceName, ushort count)
    {
      var scaledServices = new Dictionary<string, ushort> { { serviceName, count } };
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(scaledServices: scaledServices));
    }

    /// <summary>
    /// Exposes a Docker Compose service port.
    /// </summary>
    /// <remarks>
    /// The service port does not need to be published (bound to a host port) in the
    /// Docker Compose file. An ambassador container proxies the service port and
    /// makes it accessible to the test host.
    ///
    /// The readiness check waits until the service port accepts connections. It
    /// runs inside the ambassador container, and does not require anything from the
    /// service image.
    ///
    /// The service name addresses the first instance of the service. Use
    /// <see cref="WithExposedServiceInstance(string, ushort, ushort)" /> to address
    /// one container of a service that runs more than one.
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="port">The Docker Compose service port.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithExposedService(string serviceName, ushort port)
    {
      return WithExposedServiceInstance(serviceName, ComposeServiceName.FirstInstance, port);
    }

    /// <summary>
    /// Exposes a Docker Compose service port.
    /// </summary>
    /// <remarks>
    /// The service is ready when the service port accepts connections and the wait
    /// strategy indicates readiness.
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="port">The Docker Compose service port.</param>
    /// <param name="waitStrategy">The wait strategy that indicates the readiness of the service.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithExposedService(string serviceName, ushort port, IWaitForContainerOS waitStrategy)
    {
      return WithExposedServiceInstance(serviceName, ComposeServiceName.FirstInstance, port, waitStrategy);
    }

    /// <summary>
    /// Exposes the port of a Docker Compose service instance.
    /// </summary>
    /// <remarks>
    /// Use this member to address one container of a Docker Compose service that
    /// runs more than one (see <see cref="WithScaledService" />).
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <param name="port">The Docker Compose service port.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithExposedServiceInstance(string serviceName, ushort instance, ushort port)
    {
      var exposedServices = new[] { new ComposeExposedService(serviceName, instance, port) };
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(exposedServices: exposedServices));
    }

    /// <summary>
    /// Exposes the port of a Docker Compose service instance.
    /// </summary>
    /// <remarks>
    /// Use this member to address one container of a Docker Compose service that
    /// runs more than one (see <see cref="WithScaledService" />).
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <param name="port">The Docker Compose service port.</param>
    /// <param name="waitStrategy">The wait strategy that indicates the readiness of the service.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithExposedServiceInstance(string serviceName, ushort instance, ushort port, IWaitForContainerOS waitStrategy)
    {
      return WithExposedServiceInstance(serviceName, instance, port)
        .WaitingForInstance(serviceName, instance, waitStrategy);
    }

    /// <summary>
    /// Sets the wait strategy that indicates the readiness of a Docker Compose
    /// service.
    /// </summary>
    /// <remarks>
    /// In contrast to <see cref="WithExposedService(string, ushort)" />, this does
    /// not expose a service port. Use it to wait for a service that the test host
    /// does not need to reach, such as a database that only another service
    /// connects to.
    ///
    /// The service name addresses the first instance of the service. Use
    /// <see cref="WaitingForInstance" /> to address one container of a service that
    /// runs more than one.
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="waitStrategy">The wait strategy that indicates the readiness of the service.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WaitingFor(string serviceName, IWaitForContainerOS waitStrategy)
    {
      return WaitingForInstance(serviceName, ComposeServiceName.FirstInstance, waitStrategy);
    }

    /// <summary>
    /// Sets the wait strategy that indicates the readiness of a Docker Compose
    /// service instance.
    /// </summary>
    /// <remarks>
    /// Use this member to address one container of a Docker Compose service that
    /// runs more than one (see <see cref="WithScaledService" />).
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <param name="waitStrategy">The wait strategy that indicates the readiness of the service.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WaitingForInstance(string serviceName, ushort instance, IWaitForContainerOS waitStrategy)
    {
      var serviceReadiness = new[] { new ComposeServiceReadiness(serviceName, instance, waitStrategy.Build()) };
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(serviceReadiness: serviceReadiness));
    }

    /// <summary>
    /// Sets whether the Docker Compose images are pulled before the services start
    /// or not.
    /// </summary>
    /// <remarks>
    /// Enabled by default. The images are pulled from the test host instead of from
    /// inside the container, so that the Docker credentials and credential helpers
    /// of the test host apply. Docker Compose cannot use them, it does not have
    /// access to the Docker configuration of the test host.
    ///
    /// Images that are already present on the Docker host are not pulled again, and
    /// a failing pull does not fail the start. Docker Compose still tries to pull
    /// the image afterward.
    /// </remarks>
    /// <param name="pull">Determines whether the Docker Compose images are pulled before the services start or not.</param>
    /// <returns>A configured instance of <see cref="ComposeBuilder" />.</returns>
    public ComposeBuilder WithPull(bool pull)
    {
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(pull: pull));
    }

    /// <inheritdoc />
    public override ComposeContainer Build()
    {
      Validate();

      // Keep the project name short. Docker Compose prefixes the container names with
      // it, and a DNS label is limited to 63 characters.
      var projectName = $"{DockerResourceConfiguration.ProjectNamePrefix ?? DefaultProjectNamePrefix}-{Guid.NewGuid().ToString("D").Substring(0, ProjectNameSuffixLength)}";

      var composeFiles = GetComposeFilePaths();

      var containerComposeFilePaths = composeFiles.Select(GetContainerPath).ToArray();

      // Docker Compose splits COMPOSE_FILE on the path separator. Use a path
      // separator that none of the Docker Compose file paths contains, otherwise
      // Docker Compose reads a single path as multiple Docker Compose files.
      var pathSeparator = GetPathSeparator(containerComposeFilePaths);

      var composeFile = string.Join(pathSeparator, containerComposeFilePaths);

      // Docker Compose resolves relative references, such as bind mount sources,
      // against the directory of the first Docker Compose file.
      var projectDirectoryPath = GetContainerPath(Path.GetDirectoryName(composeFiles[0]));

      var fileCopyInclusions = GetFileCopyInclusionPaths();

      // Without inclusions, the entire directory of each Docker Compose file is
      // copied. With inclusions, only the inclusions and the Docker Compose files
      // themselves are copied. Docker Compose cannot read the Docker Compose files
      // otherwise.
      var resourcePaths = fileCopyInclusions.Length == 0
        ? composeFiles.Select(Path.GetDirectoryName).Distinct()
        : composeFiles.Concat(fileCopyInclusions);

      var composeBuilder = Merge(DockerResourceConfiguration, new ComposeConfiguration(projectName: projectName))
        .WithEnvironment("COMPOSE_PROJECT_NAME", projectName)
        .WithEnvironment("COMPOSE_PATH_SEPARATOR", pathSeparator)
        .WithEnvironment("COMPOSE_FILE", composeFile)
        .WithWorkingDirectory(projectDirectoryPath)
        .WithMount(new UnixSocketMount(DockerResourceConfiguration.DockerEndpointAuthConfig.Endpoint));

      composeBuilder = resourcePaths.Select(GetResourceMapping)
        .Aggregate(composeBuilder, (builder, resourceMapping) => builder.WithResourceMapping(resourceMapping));

      return new ComposeContainer(composeBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ComposeBuilder Init()
    {
      return base.Init()
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand("trap 'exit 0' TERM; sleep infinity & wait $!")
        .WithPull(true);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
      base.Validate();

      const string reuseNotSupported = "Reuse cannot be used in conjunction with the Docker Compose builder.";
      _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration.Reuse))
        .ThrowIf(argument => argument.Value.Reuse.HasValue && argument.Value.Reuse.Value, argument => new ArgumentException(reuseNotSupported, argument.Name));

      var composeFiles = GetComposeFilePaths();

      const string composeFileNotSet = "At least one Docker Compose file must be set.";
      _ = Guard.Argument(composeFiles, nameof(DockerResourceConfiguration.ComposeFiles))
        .ThrowIf(argument => argument.Value.Length == 0, argument => new ArgumentException(composeFileNotSet, argument.Name));

      const string composeFileDoesNotExist = "The Docker Compose file '{0}' does not exist.";
      _ = Guard.Argument(composeFiles, nameof(DockerResourceConfiguration.ComposeFiles))
        .ThrowIf(argument => argument.Value.Any(filePath => !File.Exists(filePath)), argument => new FileNotFoundException(string.Format(composeFileDoesNotExist, argument.Value.First(filePath => !File.Exists(filePath)))));

      const string composeFileOnUncPath = "The Docker Compose file '{0}' is on a UNC path. The Docker Compose files must be on a local drive because the Docker daemon cannot resolve a UNC path.";
      _ = Guard.Argument(composeFiles, nameof(DockerResourceConfiguration.ComposeFiles))
        .ThrowIf(argument => argument.Value.Any(IsUncPath), argument => new ArgumentException(string.Format(composeFileOnUncPath, argument.Value.First(IsUncPath)), argument.Name));

      const string composeFilePathSeparatorNotFound = "The Docker Compose file paths contain every supported path separator character ({0}). At least one of these characters must not occur in any Docker Compose file path because Docker Compose separates the paths with it.";
      _ = Guard.Argument(composeFiles, nameof(DockerResourceConfiguration.ComposeFiles))
        .ThrowIf(argument => argument.Value.Length > 0 && GetPathSeparator(argument.Value.Select(GetContainerPath)) == null, argument => new ArgumentException(string.Format(composeFilePathSeparatorNotFound, string.Join(" ", PathSeparators)), argument.Name));

      const string fileCopyInclusionDoesNotExist = "The file or directory '{0}' does not exist.";
      _ = Guard.Argument(GetFileCopyInclusionPaths(), nameof(DockerResourceConfiguration.FileCopyInclusions))
        .ThrowIf(argument => argument.Value.Any(IsFileCopyInclusionMissing), argument => new FileNotFoundException(string.Format(fileCopyInclusionDoesNotExist, argument.Value.First(IsFileCopyInclusionMissing))));

      const string projectNamePrefixTooLong = "The Docker Compose project name prefix must not exceed {0} characters because Docker Compose prefixes the container names with the project name, and a DNS label is limited to 63 characters.";
      _ = Guard.Argument(DockerResourceConfiguration.ProjectNamePrefix, nameof(DockerResourceConfiguration.ProjectNamePrefix))
        .ThrowIf(argument => argument.Value != null && argument.Value.Length > ProjectNamePrefixMaxLength, argument => new ArgumentException(string.Format(projectNamePrefixTooLong, ProjectNamePrefixMaxLength), argument.Name));

      const string projectNamePrefixInvalid = "The Docker Compose project name prefix must start with a lowercase letter or digit and can only contain lowercase letters, digits, dashes, and underscores.";
      _ = Guard.Argument(DockerResourceConfiguration.ProjectNamePrefix, nameof(DockerResourceConfiguration.ProjectNamePrefix))
        .ThrowIf(argument => argument.Value != null && !ProjectNameRegex.IsMatch(argument.Value), argument => new ArgumentException(projectNamePrefixInvalid, argument.Name));
    }

    /// <inheritdoc />
    protected override ComposeBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ComposeBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
      return Merge(DockerResourceConfiguration, new ComposeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ComposeBuilder Merge(ComposeConfiguration oldValue, ComposeConfiguration newValue)
    {
      return new ComposeBuilder(new ComposeConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Gets the resolved Docker Compose file paths.
    /// </summary>
    /// <returns>The distinct, absolute Docker Compose file paths.</returns>
    private string[] GetComposeFilePaths()
    {
      return DockerResourceConfiguration.ComposeFiles.Select(Path.GetFullPath).Distinct().ToArray();
    }

    /// <summary>
    /// Gets the resolved file copy inclusion paths.
    /// </summary>
    /// <remarks>
    /// Returns an empty list if the Docker Compose files are not set. The directory
    /// of the first Docker Compose file resolves the relative inclusion paths.
    /// </remarks>
    /// <returns>The absolute file copy inclusion paths on the test host.</returns>
    private string[] GetFileCopyInclusionPaths()
    {
      var composeFiles = GetComposeFilePaths();

      if (composeFiles.Length == 0)
      {
        return Array.Empty<string>();
      }

      var projectDirectoryPath = Path.GetDirectoryName(composeFiles[0]);
      return DockerResourceConfiguration.FileCopyInclusions.Select(fileCopyInclusion => Path.GetFullPath(Path.Combine(projectDirectoryPath, fileCopyInclusion))).ToArray();
    }

    /// <summary>
    /// Gets the path inside the container that corresponds to a path on the test
    /// host.
    /// </summary>
    /// <remarks>
    /// Docker Compose resolves relative references against the directory of the
    /// Docker Compose file and passes the resolved path to the Docker daemon, which
    /// interprets it on the test host. Keeping the path inside the container equal
    /// to the path on the test host is what makes relative bind mounts work.
    /// </remarks>
    /// <param name="path">The absolute path on the test host.</param>
    /// <returns>The corresponding path inside the container.</returns>
    private static string GetContainerPath(string path)
    {
      var containerPath = Unix.Instance.NormalizePath(path);

      // Convert a Windows drive letter to the path notation that the Docker daemon
      // understands, e.g. C:/Users/Default to /c/Users/Default.
      if (containerPath.Length > 1 && char.IsLetter(containerPath[0]) && ':'.Equals(containerPath[1]))
      {
        containerPath = "/" + char.ToLowerInvariant(containerPath[0]) + containerPath.Substring(2);
      }

      return containerPath;
    }

    /// <summary>
    /// Gets the path separator that separates the Docker Compose file paths in
    /// <c>COMPOSE_FILE</c>.
    /// </summary>
    /// <remarks>
    /// Docker Compose splits <c>COMPOSE_FILE</c> on the path separator that
    /// <c>COMPOSE_PATH_SEPARATOR</c> configures. A path that contains the path
    /// separator is read as multiple Docker Compose files, which is why the path
    /// separator must not occur in any of the paths.
    /// </remarks>
    /// <param name="containerPaths">The Docker Compose file paths inside the container.</param>
    /// <returns>The first supported path separator that none of the paths contains; otherwise, null.</returns>
    [CanBeNull]
    private static string GetPathSeparator(IEnumerable<string> containerPaths)
    {
      return PathSeparators.FirstOrDefault(pathSeparator => !containerPaths.Any(containerPath => containerPath.Contains(pathSeparator)));
    }

    /// <summary>
    /// Gets the resource mapping that copies a file or directory into the container.
    /// </summary>
    /// <remarks>
    /// A directory is copied into the container directory that corresponds to it, a
    /// file into the container directory that corresponds to its parent directory.
    /// Both keep the path they have on the test host (see
    /// <see cref="GetContainerPath" />).
    /// </remarks>
    /// <param name="path">The absolute file or directory path on the test host.</param>
    /// <returns>The resource mapping that copies the file or directory into the container.</returns>
    private static IResourceMapping GetResourceMapping(string path)
    {
      // Despite its name, FileResourceMapping works for directories too, the copy
      // logic picks the strategy based on the source path at copy time.
      var targetDirectoryPath = Directory.Exists(path) ? path : Path.GetDirectoryName(path);
      return new FileResourceMapping(path, GetContainerPath(targetDirectoryPath), 0, 0, Unix.FileMode644);
    }

    /// <summary>
    /// Checks whether a path on the test host is a UNC path, e.g.
    /// <c>\\wsl$\Ubuntu\home</c>.
    /// </summary>
    /// <remarks>
    /// A UNC path has no equivalent inside the container, and the Docker daemon
    /// cannot resolve it either (see <see cref="GetContainerPath" />).
    /// </remarks>
    /// <param name="path">The absolute path on the test host.</param>
    /// <returns>True if the path is a UNC path; otherwise, false.</returns>
    private static bool IsUncPath(string path)
    {
      return path.StartsWith("\\\\", StringComparison.Ordinal);
    }

    /// <summary>
    /// Checks whether a file copy inclusion is missing on the test host.
    /// </summary>
    /// <param name="fileCopyInclusion">The absolute file copy inclusion path on the test host.</param>
    /// <returns>True if the file copy inclusion is missing; otherwise, false.</returns>
    private static bool IsFileCopyInclusionMissing(string fileCopyInclusion)
    {
      return !File.Exists(fileCopyInclusion) && !Directory.Exists(fileCopyInclusion);
    }
  }
}
