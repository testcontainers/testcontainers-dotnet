#pragma warning disable SA1402
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

  namespace Builders
  {
    [PublicAPI]
    [Obsolete("Use the ContainerBuilder class instead.")]
    public sealed class TestcontainersBuilder<TContainerEntity> : ContainerBuilder<TContainerEntity>
      where TContainerEntity : DockerContainer
    {
    }
  }
}

#pragma warning restore SA1402
#pragma warning restore SA1403
