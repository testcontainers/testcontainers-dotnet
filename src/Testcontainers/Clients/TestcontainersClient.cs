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

    public const string TestcontainersSessionIdLabel = TestcontainersLabel + ".session-id";

    private static readonly string OSRootDirectory = Path.GetPathRoot(Directory.GetCurrentDirectory());

    private static readonly IOperatingSystem OS = new Unix(dockerEndpointAuthConfig: null);

    private readonly DockerRegistryAuthenticationProvider _registryAuthenticationProvider;

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
        new DockerNetworkOperations(sessionId, dockerEndpointAuthConfig, logger),
        new DockerVolumeOperations(sessionId, dockerEndpointAuthConfig, logger),
        new DockerSystemOperations(sessionId, dockerEndpointAuthConfig, logger),
        new DockerRegistryAuthenticationProvider(logger))
    {
    }

    private TestcontainersClient(
      IDockerContainerOperations containerOperations,
      IDockerImageOperations imageOperations,
      IDockerNetworkOperations networkOperations,
      IDockerVolumeOperations volumeOperations,
      IDockerSystemOperations systemOperations,
      DockerRegistryAuthenticationProvider registryAuthenticationProvider)
    {
      _registryAuthenticationProvider = registryAuthenticationProvider;
      Container = containerOperations;
      Image = imageOperations;
      Network = networkOperations;
      Volume = volumeOperations;
      System = systemOperations;
    }

    /// <inheritdoc />
    public IDockerContainerOperations Container { get; }

    /// <inheritdoc />
    public IDockerImageOperations Image { get; }

    /// <inheritdoc />
    public IDockerNetworkOperations Network { get; }

    /// <inheritdoc />
    public IDockerVolumeOperations Volume { get; }

    /// <inheritdoc />
    public IDockerSystemOperations System { get; }

    /// <inheritdoc />
    public bool IsRunningInsideDocker => File.Exists(Path.Combine(OSRootDirectory, ".dockerenv"));

    /// <inheritdoc />
    public Task<long> GetContainerExitCodeAsync(string id, CancellationToken ct = default)
    {
      return Container.GetExitCodeAsync(id, ct);
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

      return Container.GetLogsAsync(id, since.ToUniversalTime().Subtract(unixEpoch), until.ToUniversalTime().Subtract(unixEpoch), timestampsEnabled, ct);
    }

    /// <inheritdoc />
    public Task<ContainerInspectResponse> InspectContainerAsync(string id, CancellationToken ct = default)
    {
      return Container.InspectAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task StartAsync(string id, CancellationToken ct = default)
    {
      if (await Container.ExistsWithIdAsync(id, ct)
            .ConfigureAwait(false))
      {
        await Container.StartAsync(id, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task StopAsync(string id, CancellationToken ct = default)
    {
      if (await Container.ExistsWithIdAsync(id, ct)
            .ConfigureAwait(false))
      {
        await Container.StopAsync(id, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string id, CancellationToken ct = default)
    {
      if (await Container.ExistsWithIdAsync(id, ct)
            .ConfigureAwait(false))
      {
        try
        {
          await Container.RemoveAsync(id, ct)
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
    public Task<ExecResult> ExecAsync(string id, IList<string> command, CancellationToken ct = default)
    {
      return Container.ExecAsync(id, command, ct);
    }

    /// <inheritdoc />
    public async Task CopyAsync(string id, IResourceMapping resourceMapping, CancellationToken ct = default)
    {
      using (var tarOutputMemStream = new TarOutputMemoryStream())
      {
        await tarOutputMemStream.AddAsync(resourceMapping, ct)
          .ConfigureAwait(false);

        tarOutputMemStream.Close();
        tarOutputMemStream.Seek(0, SeekOrigin.Begin);

        await Container.ExtractArchiveToContainerAsync(id, "/", tarOutputMemStream, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task CopyAsync(string id, DirectoryInfo source, string target, CancellationToken ct = default)
    {
      using (var tarOutputMemStream = new TarOutputMemoryStream(target))
      {
        await tarOutputMemStream.AddAsync(source, true, Unix.FileMode644, ct)
          .ConfigureAwait(false);

        tarOutputMemStream.Close();
        tarOutputMemStream.Seek(0, SeekOrigin.Begin);

        await Container.ExtractArchiveToContainerAsync(id, "/", tarOutputMemStream, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task CopyAsync(string id, FileInfo source, string target, CancellationToken ct = default)
    {
      using (var tarOutputMemStream = new TarOutputMemoryStream(target))
      {
        await tarOutputMemStream.AddAsync(source, Unix.FileMode644, ct)
          .ConfigureAwait(false);

        tarOutputMemStream.Close();
        tarOutputMemStream.Seek(0, SeekOrigin.Begin);

        await Container.ExtractArchiveToContainerAsync(id, "/", tarOutputMemStream, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task CopyFileAsync(string id, string filePath, byte[] fileContent, int accessMode, int userId, int groupId, CancellationToken ct = default)
    {
      var containerPath = OS.NormalizePath(filePath);

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

        await Container.ExtractArchiveToContainerAsync(id, "/", tarOutputMemStream, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task<byte[]> ReadFileAsync(string id, string filePath, CancellationToken ct = default)
    {
      Stream tarStream;

      var containerPath = OS.NormalizePath(filePath);

      try
      {
        tarStream = await Container.GetArchiveFromContainerAsync(id, containerPath, ct)
          .ConfigureAwait(false);
      }
      catch (DockerContainerNotFoundException e)
      {
        throw new FileNotFoundException(null, Path.GetFileName(containerPath), e);
      }

      using (var tarInputStream = new TarInputStream(tarStream, Encoding.Default))
      {
        tarInputStream.IsStreamOwner = true;

        var entry = await tarInputStream.GetNextEntryAsync(ct)
          .ConfigureAwait(false);

        if (entry.IsDirectory)
        {
          throw new InvalidOperationException("Cannot read from a directory. Use a file instead.");
        }

        var readBytes = new byte[entry.Size];

#if NETSTANDARD2_1_OR_GREATER
        _ = await tarInputStream.ReadAsync(new Memory<byte>(readBytes), ct)
          .ConfigureAwait(false);
#else
        _ = await tarInputStream.ReadAsync(readBytes, 0, readBytes.Length, ct)
          .ConfigureAwait(false);
#endif

        return readBytes;
      }
    }

    /// <inheritdoc />
    public async Task<string> RunAsync(IContainerConfiguration configuration, CancellationToken ct = default)
    {
      if (TestcontainersSettings.ResourceReaperEnabled && ResourceReaper.DefaultSessionId.Equals(configuration.SessionId))
      {
        var isWindowsEngineEnabled = await System.GetIsWindowsEngineEnabled(ct)
          .ConfigureAwait(false);

        _ = await ResourceReaper.GetAndStartDefaultAsync(configuration.DockerEndpointAuthConfig, isWindowsEngineEnabled, ct)
          .ConfigureAwait(false);
      }

      var cachedImage = await Image.ByNameAsync(configuration.Image.FullName, ct)
        .ConfigureAwait(false);

      if (configuration.ImagePullPolicy(cachedImage))
      {
        var dockerRegistryServerAddress = configuration.Image.GetHostname();

        if (dockerRegistryServerAddress == null)
        {
          var info = await System.GetInfoAsync(ct)
            .ConfigureAwait(false);

          dockerRegistryServerAddress = info.IndexServerAddress;
        }

        var authConfig = _registryAuthenticationProvider.GetAuthConfig(dockerRegistryServerAddress);

        await Image.CreateAsync(configuration.Image, authConfig, ct)
          .ConfigureAwait(false);
      }

      var id = await Container.RunAsync(configuration, ct)
        .ConfigureAwait(false);

      if (configuration.Networks.Any() && PortForwardingContainer.Instance != null && TestcontainersStates.Running.Equals(PortForwardingContainer.Instance.State))
      {
        await Network.ConnectAsync("bridge", id, ct)
          .ConfigureAwait(false);
      }

      if (configuration.ResourceMappings.Any())
      {
        await Task.WhenAll(configuration.ResourceMappings.Values.Select(resourceMapping => CopyAsync(id, resourceMapping, ct)))
          .ConfigureAwait(false);
      }

      return id;
    }

    /// <inheritdoc />
    public async Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default)
    {
      var cachedImage = await Image.ByNameAsync(configuration.Image.FullName, ct)
        .ConfigureAwait(false);

      if (configuration.ImageBuildPolicy(cachedImage))
      {
        _ = await Image.BuildAsync(configuration, ct)
          .ConfigureAwait(false);
      }

      return configuration.Image.FullName;
    }

    /// <summary>
    /// Represent a tar archive file.
    /// </summary>
    private sealed class TarOutputMemoryStream : TarOutputStream
    {
      private readonly string _targetDirectoryPath;

      /// <summary>
      /// Initializes a new instance of the <see cref="TarOutputMemoryStream" /> class.
      /// </summary>
      /// <param name="targetDirectoryPath">The target directory path to extract the files to.</param>
      public TarOutputMemoryStream(string targetDirectoryPath)
        : this()
      {
        _targetDirectoryPath = targetDirectoryPath;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TarOutputMemoryStream" /> class.
      /// </summary>
      public TarOutputMemoryStream()
        : base(new MemoryStream(), Encoding.Default)
      {
        IsStreamOwner = false;
      }

      /// <summary>
      /// Adds the content of an implementation of <see cref="IResourceMapping" /> to the archive.
      /// </summary>
      /// <param name="resourceMapping">The resource mapping to add to the archive.</param>
      /// <param name="ct">Cancellation token.</param>
      public async Task AddAsync(IResourceMapping resourceMapping, CancellationToken ct = default)
      {
        var fileContent = await resourceMapping.GetAllBytesAsync(ct)
          .ConfigureAwait(false);

        var targetFilePath = OS.NormalizePath(resourceMapping.Target);

        var tarEntry = new TarEntry(new TarHeader());
        tarEntry.TarHeader.Name = targetFilePath;
        tarEntry.TarHeader.Mode = (int)resourceMapping.FileMode;
        tarEntry.TarHeader.ModTime = DateTime.UtcNow;
        tarEntry.Size = fileContent.Length;

        await PutNextEntryAsync(tarEntry, ct)
          .ConfigureAwait(false);

#if NETSTANDARD2_1_OR_GREATER
        await WriteAsync(fileContent, ct)
          .ConfigureAwait(false);
#else
        await WriteAsync(fileContent, 0, fileContent.Length, ct)
          .ConfigureAwait(false);
#endif

        await CloseEntryAsync(ct)
          .ConfigureAwait(false);
      }

      /// <summary>
      /// Adds a file to the archive.
      /// </summary>
      /// <param name="file">The file to add to the archive.</param>
      /// <param name="fileMode">The POSIX file mode permission.</param>
      /// <param name="ct">Cancellation token.</param>
      /// <returns>A task that completes when the file has been added to the archive.</returns>
      public Task AddAsync(FileInfo file, UnixFileMode fileMode = Unix.FileMode644, CancellationToken ct = default)
      {
        return AddAsync(file.Directory, file, fileMode, ct);
      }

      /// <summary>
      /// Adds a directory to the archive.
      /// </summary>
      /// <param name="directory">The directory to add to the archive.</param>
      /// <param name="recurse">A value indicating whether the current directory and all its subdirectories are included or not.</param>
      /// <param name="fileMode">The POSIX file mode permission.</param>
      /// <param name="ct">Cancellation token.</param>
      public async Task AddAsync(DirectoryInfo directory, bool recurse = true, UnixFileMode fileMode = Unix.FileMode644, CancellationToken ct = default)
      {
        var searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        foreach (var file in directory.GetFiles("*", searchOption))
        {
          await AddAsync(directory, file, fileMode, ct)
            .ConfigureAwait(false);
        }
      }

      /// <summary>
      /// Adds a file to the archive.
      /// </summary>
      /// <param name="directory">The root directory of the file to add to the archive.</param>
      /// <param name="file">The file to add to the archive.</param>
      /// <param name="fileMode">The POSIX file mode permission.</param>
      /// <param name="ct">Cancellation token.</param>
      public async Task AddAsync(DirectoryInfo directory, FileInfo file, UnixFileMode fileMode = Unix.FileMode644, CancellationToken ct = default)
      {
        using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
        {
          var targetFilePath = OS.NormalizePath(Path.Combine(_targetDirectoryPath, file.FullName.Substring(directory.FullName.Length + 1)));

          var tarEntry = new TarEntry(new TarHeader());
          tarEntry.TarHeader.Name = targetFilePath;
          tarEntry.TarHeader.Mode = (int)fileMode;
          tarEntry.TarHeader.ModTime = file.LastWriteTimeUtc;
          tarEntry.Size = stream.Length;

          await PutNextEntryAsync(tarEntry, ct)
            .ConfigureAwait(false);

          await stream.CopyToAsync(this, 81920, ct)
            .ConfigureAwait(false);

          await CloseEntryAsync(ct)
            .ConfigureAwait(false);
        }
      }
    }
  }
}
