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
  using Org.BouncyCastle.OpenSsl;
  using Xunit;

  public abstract class ProtectDockerDaemonSocket : IAsyncLifetime
  {
    private const string CertsDirectoryName = "certs";

    private const ushort TlsPort = 2376;

    private readonly string _hostCertsDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"), CertsDirectoryName);

    private readonly string _containerCertsDirectoryPath = Path.Combine("/", CertsDirectoryName);

    private readonly IContainer _container;

    protected ProtectDockerDaemonSocket(ContainerBuilder containerConfiguration, string dockerImageVersion)
    {
      _container = containerConfiguration
        .WithImage(new DockerImage("docker", null, dockerImageVersion + "-dind"))
        .WithPrivileged(true)
        .WithPortBinding(TlsPort, true)
        .WithBindMount(_hostCertsDirectoryPath, _containerCertsDirectoryPath, AccessMode.ReadWrite)
        .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new UntilListenOn()))
        .Build();
    }

    public virtual IList<string> CustomProperties
    {
      get
      {
        var customProperties = new List<string>();
        customProperties.Add($"docker.host={new UriBuilder("tcp", _container.Hostname, _container.GetMappedPublicPort(TlsPort))}");
        customProperties.Add($"docker.cert.path={Path.Combine(_hostCertsDirectoryPath, "client")}");
        return customProperties;
      }
    }

    public IImage Image
    {
      get
      {
        return _container.Image;
      }
    }

    public object TlsKey
    {
      get
      {
        using (var tlsKeyStream = new StreamReader(Path.Combine(_hostCertsDirectoryPath, "client", "key.pem")))
        {
          return new PemReader(tlsKeyStream).ReadObject();
        }
      }
    }

    public Task InitializeAsync()
    {
      _ = Directory.CreateDirectory(_hostCertsDirectoryPath);
      return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return _container.DisposeAsync().AsTask();
    }

    private sealed class UntilListenOn : IWaitUntil
    {
      public async Task<bool> UntilAsync(IContainer container)
      {
        var (_, stderr) = await container.GetLogsAsync()
          .ConfigureAwait(false);

        return stderr != null && stderr.Contains("API listen on [::]:2376");
      }
    }
  }
}
