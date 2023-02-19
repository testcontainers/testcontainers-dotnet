#pragma warning disable SA1403

namespace DotNet.Testcontainers
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  namespace Containers
  {
    [PublicAPI]
    [Obsolete("Use the IContainer interface instead.")]
    public interface ITestcontainersContainer : IDockerContainer
    {
    }

    [PublicAPI]
    [Obsolete("Use the IContainer interface instead.")]
    public interface IDockerContainer : IAsyncDisposable
    {
      [NotNull]
      ILogger Logger { get; }

      [NotNull]
      string Id { get; }

      [NotNull]
      string Name { get; }

      [NotNull]
      string IpAddress { get; }

      [NotNull]
      string MacAddress { get; }

      [NotNull]
      string Hostname { get; }

      [NotNull]
      IImage Image { get; }

      TestcontainersStates State { get; }

      TestcontainersHealthStatus Health { get; }

      long HealthCheckFailingStreak { get; }

      ushort GetMappedPublicPort(int containerPort);

      ushort GetMappedPublicPort(string containerPort);

      [Obsolete("Use IContainer.GetExitCodeAsync(CancellationToken) instead.")]
      Task<long> GetExitCode(CancellationToken ct = default);

      Task<long> GetExitCodeAsync(CancellationToken ct = default);

      [Obsolete("Use IContainer.GetLogsAsync(DateTime, DateTime, bool, CancellationToken) instead.")]
      Task<(string Stdout, string Stderr)> GetLogs(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default);

      Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default);

      Task StartAsync(CancellationToken ct = default);

      Task StopAsync(CancellationToken ct = default);

      Task CopyFileAsync(string filePath, byte[] fileContent, int accessMode = 384, int userId = 0, int groupId = 0, CancellationToken ct = default);

      Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default);

      Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default);
    }

    [PublicAPI]
    [Obsolete("Use the DockerContainer class instead.")]
    public sealed class TestcontainersContainer : DockerContainer
    {
      internal TestcontainersContainer(IContainerConfiguration configuration, ILogger logger)
        : base(configuration, logger)
      {
      }
    }
  }

  namespace Images
  {
    [PublicAPI]
    [Obsolete("Use the IImage interface instead.")]
    public interface IDockerImage
    {
      [NotNull]
      string Repository { get; }

      [NotNull]
      string Name { get; }

      [NotNull]
      string Tag { get; }

      [NotNull]
      string FullName { get; }

      [CanBeNull]
      string GetHostname();
    }

    /// <summary>
    /// Maps the old to the new interface to provide backwards compatibility.
    /// </summary>
    public sealed partial class DockerImage
    {
      public DockerImage(IDockerImage image)
        : this(image.Repository, image.Name, image.Tag)
      {
      }
    }
  }

  namespace Networks
  {
    [PublicAPI]
    [Obsolete("Use the INetwork interface instead.")]
    public interface IDockerNetwork
    {
      [NotNull]
      string Name { get; }

      Task CreateAsync(CancellationToken ct = default);

      Task DeleteAsync(CancellationToken ct = default);
    }

    /// <summary>
    /// Maps the old to the new interface to provide backwards compatibility.
    /// </summary>
    internal sealed partial class DockerNetwork
    {
      public DockerNetwork(IDockerNetwork network)
      {
        this.network.Name = network.Name;
      }
    }
  }

  namespace Volumes
  {
    [PublicAPI]
    [Obsolete("Use the IVolume interface instead.")]
    public interface IDockerVolume
    {
      [NotNull]
      string Name { get; }

      Task CreateAsync(CancellationToken ct = default);

      Task DeleteAsync(CancellationToken ct = default);
    }

    /// <summary>
    /// Maps the old to the new interface to provide backwards compatibility.
    /// </summary>
    internal sealed partial class DockerVolume
    {
      public DockerVolume(IDockerVolume volume)
      {
        this.volume.Name = volume.Name;
      }
    }
  }

  namespace Builders
  {
    [PublicAPI]
    [Obsolete("Use the ContainerBuilder class instead.")]
    public sealed class TestcontainersBuilder<TContainerEntity> : ContainerBuilder<TContainerEntity>
      where TContainerEntity : DockerContainer
    {
    }

    [PublicAPI]
    [Obsolete("Use the NetworkBuilder class instead.")]
    public sealed class TestcontainersNetworkBuilder : NetworkBuilder
    {
    }

    [PublicAPI]
    [Obsolete("Use the VolumeBuilder class instead.")]
    public sealed class TestcontainersVolumeBuilder : VolumeBuilder
    {
    }
  }
}

#pragma warning restore SA1403
