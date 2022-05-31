namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.IO;
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
    public const string TestcontainersLabel = "dotnet.testcontainers";

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
        TestcontainersSettings.OS.DockerApiEndpoint,
        TestcontainersSettings.Logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersClient" /> class.
    /// </summary>
    /// <param name="endpoint">The Docker API endpoint.</param>
    /// <param name="logger">The logger.</param>
    public TestcontainersClient(Uri endpoint, ILogger logger)
      : this(
        new DockerContainerOperations(endpoint, logger),
        new DockerImageOperations(endpoint, logger),
        new DockerSystemOperations(endpoint, logger),
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
    public Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default)
    {
      return this.system.GetIsWindowsEngineEnabled(ct);
    }

    /// <inheritdoc />
    public Task<ContainerListResponse> GetContainer(string id, CancellationToken ct = default)
    {
      return this.containers.ByIdAsync(id, ct);
    }

    /// <inheritdoc />
    public Task<ContainerInspectResponse> InspectContainer(string id, CancellationToken ct = default)
    {
      return this.containers.InspectAsync(id, ct);
    }

    /// <inheritdoc />
    public Task<long> GetContainerExitCode(string id, CancellationToken ct = default)
    {
      return this.containers.GetExitCode(id, ct);
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
      using (var memStream = new MemoryStream())
      {
        using (var tarOutputStream = new TarOutputStream(memStream, Encoding.Default))
        {
          tarOutputStream.IsStreamOwner = false;
          tarOutputStream.PutNextEntry(
            new TarEntry(
              new TarHeader
              {
                Name = filePath,
                UserId = userId,
                GroupId = groupId,
                Mode = accessMode,
                Size = fileContent.Length,
              }));

#if NETSTANDARD2_1_OR_GREATER
          await tarOutputStream.WriteAsync(fileContent.AsMemory(0, fileContent.Length), ct)
            .ConfigureAwait(false);
#else
          await tarOutputStream.WriteAsync(fileContent, 0, fileContent.Length, ct)
            .ConfigureAwait(false);
#endif

          tarOutputStream.CloseEntry();
        }

        memStream.Seek(0, SeekOrigin.Begin);

        await this.containers.ExtractArchiveToContainerAsync(id, "/", memStream, ct)
          .ConfigureAwait(false);
      }
    }

    /// <inheritdoc />
    public async Task<string> RunAsync(ITestcontainersConfiguration configuration, CancellationToken ct = default)
    {
      // TODO: Workaround until we have a Windows Docker image of Ryuk
      var isWindowsEngineEnabled = await this.GetIsWindowsEngineEnabled(ct)
        .ConfigureAwait(false);

      if (!isWindowsEngineEnabled && ResourceReaper.DefaultSessionId.ToString("D").Equals(configuration.Labels[ResourceReaper.ResourceReaperSessionLabel], StringComparison.OrdinalIgnoreCase))
      {
        _ = await ResourceReaper.GetAndStartDefaultAsync(configuration.Endpoint?.ToString(), ct)
          .ConfigureAwait(false);
      }

      if (!await this.images.ExistsWithNameAsync(configuration.Image.FullName, ct)
        .ConfigureAwait(false))
      {
        var authConfig = !default(DockerRegistryAuthenticationConfiguration).Equals(configuration.DockerRegistryAuthConfig)
          ? configuration.DockerRegistryAuthConfig : this.registryAuthenticationProvider.GetAuthConfig(configuration.Image.GetHostname());

        await this.images.CreateAsync(configuration.Image, authConfig, ct)
          .ConfigureAwait(false);
      }

      var id = await this.containers.RunAsync(configuration, ct)
        .ConfigureAwait(false);

      return id;
    }

    /// <inheritdoc />
    public Task<string> BuildAsync(IImageFromDockerfileConfiguration configuration, CancellationToken ct = default)
    {
      return this.images.BuildAsync(configuration, ct);
    }
  }
}
