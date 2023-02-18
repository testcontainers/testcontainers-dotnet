namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using ICSharpCode.SharpZipLib.Tar;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="ITestcontainersClient" />
  internal sealed class TestcontainersClient : ITestcontainersClient
  {
    public const string TestcontainersLabel = "org.testcontainers";

    public const string TestcontainersLangLabel = TestcontainersLabel + ".lang";

    public const string TestcontainersVersionLabel = TestcontainersLabel + ".version";

    private readonly string osRootDirectory = Path.GetPathRoot(Directory.GetCurrentDirectory());

    private readonly IDockerContainerOperations containers;

    private readonly IDockerImageOperations images;

    private readonly IDockerSystemOperations system;

    private readonly DockerRegistryAuthenticationProvider registryAuthenticationProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersClient" /> class.
    /// </summary>
    public TestcontainersClient()
      : this(
        Guid.Empty,
        TestcontainersSettings.OS.DockerEndpointAuthConfig,
        TestcontainersSettings.Logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersClient" /> class.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    /// <param name="dockerEndpointAuthConfig">The Docker endpoint authentication configuration.</param>
    /// <param name="logger">The logger.</param>
    public TestcontainersClient(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : this(
        new DockerContainerOperations(sessionId, dockerEndpointAuthConfig, logger),
        new DockerImageOperations(sessionId, dockerEndpointAuthConfig, logger),
        new DockerSystemOperations(sessionId, dockerEndpointAuthConfig, logger),
        new DockerRegistryAuthenticationProvider(logger))
    {
    }

    private TestcontainersClient(
      IDockerContainerOperations containerOperations,
      IDockerImageOperations imageOperations,
      IDockerSystemOperations systemOperations,
      DockerRegistryAuthenticationProvider registryAuthenticationProvider)
    {
      this.containers = containerOperations;
      this.images = imageOperations;
      this.system = systemOperations;
      this.registryAuthenticationProvider = registryAuthenticationProvider;
    }

    /// <inheritdoc />
    public bool IsRunningInsideDocker
    {
      get
      {
        return File.Exists(Path.Combine(this.osRootDirectory, ".dockerenv"));
      }
    }

    /// <inheritdoc />
    public Task<long> GetContainerExitCodeAsync(string id, CancellationToken ct = default)
    {
      return this.containers.GetExitCodeAsync(id, ct);
    }

    /// <inheritdoc />
    public Task<(string Stdout, string Stderr)> GetContainerLogsAsync(string id, DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default)
    {
      var unixEpoch = new DateTime(1970, 1, 1);

      if (default(DateTime).Equals(since))
      {
        since = DateTime.MinValue;
      }

      if (default(DateTime).Equals(until))
      {
        until = DateTime.MaxValue;
      }

      return this.containers.GetLogsAsync(id, since.ToUniversalTime().Subtract(unixEpoch), until.ToUniversalTime().Subtract(unixEpoch), timestampsEnabled, ct);
    }

    /// <inheritdoc />
    public Task<ContainerInspectResponse> InspectContainerAsync(string id, CancellationToken ct = default)
    {
      return this.containers.InspectAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task StartAsync(string id, CancellationToken ct = default)
    {
      if (await this.containers.ExistsWithIdAsync(id, ct)
            .ConfigureAwait(false))
      {
        await this.containers.StartAsync(id, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task StopAsync(string id, CancellationToken ct = default)
    {
      if (await this.containers.ExistsWithIdAsync(id, ct)
            .ConfigureAwait(false))
      {
        await this.containers.StopAsync(id, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string id, CancellationToken ct = default)
    {
      if (await this.containers.ExistsWithIdAsync(id, ct)
            .ConfigureAwait(false))
      {
        try
        {
          await this.containers.RemoveAsync(id, ct)
            .ConfigureAwait(false);
        }
        catch (DockerApiException e)
        {
          // The Docker daemon may already start the progress to removes the container (AutoRemove):
          // https://docs.docker.com/engine/api/v1.41/#operation/ContainerCreate.
          if (!e.Message.Contains($"removal of container {id} is already in progress"))
          {
            throw;
          }
        }
      }
    }

    /// <inheritdoc />
    public Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default)
    {
      return this.containers.AttachAsync(id, outputConsumer, ct);
    }

    /// <inheritdoc />
    public Task<ExecResult> ExecAsync(string id, IList<string> command, CancellationToken ct = default)
    {
      return this.containers.ExecAsync(id, command, ct);
    }

    /// <inheritdoc />
    public async Task CopyFileAsync(string id, string filePath, byte[] fileContent, int accessMode, int userId, int groupId, CancellationToken ct = default)
    {
      IOperatingSystem os = new Unix(dockerEndpointAuthConfig: null);
      var containerPath = os.NormalizePath(filePath);

      using (var tarOutputMemStream = new MemoryStream())
      {
        using (var tarOutputStream = new TarOutputStream(tarOutputMemStream, Encoding.Default))
        {
          tarOutputStream.IsStreamOwner = false;

          var header = new TarHeader();
          header.Name = containerPath;
          header.UserId = userId;
          header.GroupId = groupId;
          header.Mode = accessMode;
          header.Size = fileContent.Length;

          var entry = new TarEntry(header);

          await tarOutputStream.PutNextEntryAsync(entry, ct)
            .ConfigureAwait(false);

#if NETSTANDARD2_1_OR_GREATER
          await tarOutputStream.WriteAsync(fileContent, ct)
            .ConfigureAwait(false);
#else
          await tarOutputStream.WriteAsync(fileContent, 0, fileContent.Length, ct)
            .ConfigureAwait(false);
#endif

          await tarOutputStream.CloseEntryAsync(ct)
            .ConfigureAwait(false);
        }

        tarOutputMemStream.Seek(0, SeekOrigin.Begin);

        await this.containers.ExtractArchiveToContainerAsync(id, Path.AltDirectorySeparatorChar.ToString(), tarOutputMemStream, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task<byte[]> ReadFileAsync(string id, string filePath, CancellationToken ct = default)
    {
      Stream tarStream;

      IOperatingSystem os = new Unix(dockerEndpointAuthConfig: null);
      var containerPath = os.NormalizePath(filePath);

      try
      {
        tarStream = await this.containers.GetArchiveFromContainerAsync(id, containerPath, ct)
          .ConfigureAwait(false);
      }
      catch (DockerContainerNotFoundException e)
      {
        throw new FileNotFoundException(null, Path.GetFileName(containerPath), e);
      }

      using (var tarInputStream = new TarInputStream(tarStream, Encoding.Default))
      {
        tarInputStream.IsStreamOwner = true;

        var entry = tarInputStream.GetNextEntry();

        if (entry.IsDirectory)
        {
          throw new InvalidOperationException("Cannot read from a directory. Use a file instead.");
        }

        var content = new byte[entry.Size];

        // Calling ReadAsync will not work reliably because of some internal buffering in SharpZipLib. This might very well change in future versions of SharpZipLib.
        _ = tarInputStream.Read(content, 0, content.Length);
        return content;
      }
    }

    /// <inheritdoc />
    public async Task<string> RunAsync(IContainerConfiguration configuration, CancellationToken ct = default)
    {
      async Task CopyResourceMapping(string containerId, IResourceMapping resourceMapping)
      {
        var resourceMappingContent = await resourceMapping.GetAllBytesAsync(ct)
          .ConfigureAwait(false);

        await this.CopyFileAsync(containerId, resourceMapping.Target, resourceMappingContent, 420, 0, 0, ct)
          .ConfigureAwait(false);
      }

      if (TestcontainersSettings.ResourceReaperEnabled && ResourceReaper.DefaultSessionId.Equals(configuration.SessionId))
      {
        _ = await ResourceReaper.GetAndStartDefaultAsync(configuration.DockerEndpointAuthConfig, ct)
          .ConfigureAwait(false);
      }

      var cachedImage = await this.images.ByNameAsync(configuration.Image.FullName, ct)
        .ConfigureAwait(false);

      if (configuration.ImagePullPolicy(cachedImage))
      {
        var dockerRegistryServerAddress = configuration.Image.GetHostname();

        if (dockerRegistryServerAddress == null)
        {
          var info = await this.system.GetInfoAsync(ct)
            .ConfigureAwait(false);

          dockerRegistryServerAddress = info.IndexServerAddress;
        }

        var authConfig = this.registryAuthenticationProvider.GetAuthConfig(dockerRegistryServerAddress);

        await this.images.CreateAsync(configuration.Image, authConfig, ct)
          .ConfigureAwait(false);
      }

      var id = await this.containers.RunAsync(configuration, ct)
        .ConfigureAwait(false);

      if (configuration.ResourceMappings != null)
      {
        await Task.WhenAll(configuration.ResourceMappings.Values.Select(resourceMapping => CopyResourceMapping(id, resourceMapping)))
          .ConfigureAwait(false);
      }

      return id;
    }

    /// <inheritdoc />
    public Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default)
    {
      return this.images.BuildAsync(configuration, ct);
    }
  }
}
