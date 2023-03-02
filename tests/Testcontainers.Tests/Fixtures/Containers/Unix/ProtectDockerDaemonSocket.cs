namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using Xunit;

  public abstract class ProtectDockerDaemonSocket : IAsyncLifetime
  {
    public const string DockerVersion = "20.10.18";

    private const string CertsDirectoryName = "certs";

    private const ushort TlsPort = 2376;

    private readonly string hostCertsDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"), CertsDirectoryName);

    private readonly string containerCertsDirectoryPath = Path.Combine("/", CertsDirectoryName);

    private readonly IImage image = new DockerImage(string.Empty, "docker", DockerVersion + "-dind");

    private readonly ITestcontainersContainer container;

    protected ProtectDockerDaemonSocket(ContainerBuilder<TestcontainersContainer> containerConfiguration)
    {
      this.container = containerConfiguration
        .WithImage(this.image)
        .WithPrivileged(true)
        .WithExposedPort(TlsPort)
        .WithPortBinding(TlsPort, true)
        .WithBindMount(this.hostCertsDirectoryPath, this.containerCertsDirectoryPath, AccessMode.ReadWrite)
        .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new UntilListenOn()))
        .Build();
    }

    public virtual IList<string> CustomProperties
    {
      get
      {
        var customProperties = new List<string>();
        customProperties.Add($"docker.host={this.TcpEndpoint}");
        customProperties.Add($"docker.cert.path={Path.Combine(this.hostCertsDirectoryPath, "client")}");
        return customProperties;
      }
    }

    private Uri TcpEndpoint
    {
      get
      {
        return new UriBuilder("tcp", this.container.Hostname, this.container.GetMappedPublicPort(TlsPort)).Uri;
      }
    }

    public Task InitializeAsync()
    {
      _ = Directory.CreateDirectory(this.hostCertsDirectoryPath);
      return this.container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.container.DisposeAsync().AsTask();
    }

    private sealed class UntilListenOn : IWaitUntil
    {
      public async Task<bool> UntilAsync(IContainer container)
      {
        var (_, stderr) = await container.GetLogs()
          .ConfigureAwait(false);

        return stderr != null && stderr.Contains("API listen on [::]:2376");
      }
    }
  }
}
