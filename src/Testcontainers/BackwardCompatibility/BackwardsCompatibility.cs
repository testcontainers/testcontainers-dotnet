﻿namespace DotNet.Testcontainers
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

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

    Task<long> GetExitCode(CancellationToken ct = default);

    Task<(string Stdout, string Stderr)> GetLogs(DateTime since = default, DateTime until = default, CancellationToken ct = default);

    Task StartAsync(CancellationToken ct = default);

    Task StopAsync(CancellationToken ct = default);

    Task CopyFileAsync(string filePath, byte[] fileContent, int accessMode = 384, int userId = 0, int groupId = 0, CancellationToken ct = default);

    Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default);

    Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default);
  }

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

  [PublicAPI]
  [Obsolete("Use the INetwork interface instead.")]
  public interface IDockerNetwork
  {
    [NotNull]
    string Id { get; }

    [NotNull]
    string Name { get; }

    Task CreateAsync(CancellationToken ct = default);

    Task DeleteAsync(CancellationToken ct = default);
  }

  [PublicAPI]
  [Obsolete("Use the IVolume interface instead.")]
  public interface IDockerVolume
  {
    [NotNull]
    string Name { get; }

    Task CreateAsync(CancellationToken ct = default);

    Task DeleteAsync(CancellationToken ct = default);
  }

  [PublicAPI]
  [Obsolete("Use the ContainerBuilder class instead.")]
  public sealed class TestcontainersBuilder<TContainerEntity> : ContainerBuilder<TContainerEntity>
    where TContainerEntity : DockerContainer
  {
  }

  [PublicAPI]
  [Obsolete("Use the ImageBuilder class instead.")]
  public sealed class ImageFromDockerfileBuilder : ImageBuilder
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

  [PublicAPI]
  [Obsolete("Use the VolumeBuilder class instead.")]
  public sealed class TestcontainersContainer : DockerContainer
  {
    public TestcontainersContainer(IContainerConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }
  }
}