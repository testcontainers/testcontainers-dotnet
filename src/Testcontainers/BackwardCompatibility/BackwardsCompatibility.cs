#pragma warning disable SA1402, SA1403

namespace DotNet.Testcontainers
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  namespace Containers
  {
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

#pragma warning restore SA1402, SA1403
