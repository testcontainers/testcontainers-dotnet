namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Xunit;

  public static class AzuriteFixture
  {
    [UsedImplicitly]
    public class AzuriteDefaultFixture : IAsyncLifetime
    {
      public AzuriteDefaultFixture()
        : this(new AzuriteTestcontainerConfiguration())
      {
      }

      protected AzuriteDefaultFixture(AzuriteTestcontainerConfiguration configuration)
      {
        Configuration = configuration;
        Container = new TestcontainersBuilder<AzuriteTestcontainer>()
          .WithAzurite(configuration)
          .Build();
      }

      public AzuriteTestcontainerConfiguration Configuration { get; }

      public AzuriteTestcontainer Container { get; }

      public Task InitializeAsync()
      {
        return Container.StartAsync();
      }

      public Task DisposeAsync()
      {
        return Container.DisposeAsync().AsTask();
      }
    }

    [UsedImplicitly]
    public sealed class AzuriteWithBlobOnlyFixture : AzuriteDefaultFixture
    {
      public AzuriteWithBlobOnlyFixture()
        : base(new AzuriteTestcontainerConfiguration { BlobServiceOnlyEnabled = true })
      {
      }
    }

    [UsedImplicitly]
    public sealed class AzuriteWithQueueOnlyFixture : AzuriteDefaultFixture
    {
      public AzuriteWithQueueOnlyFixture()
        : base(new AzuriteTestcontainerConfiguration { QueueServiceOnlyEnabled = true })
      {
      }
    }

    [UsedImplicitly]
    public sealed class AzuriteWithTableOnlyFixture : AzuriteDefaultFixture
    {
      public AzuriteWithTableOnlyFixture()
        : base(new AzuriteTestcontainerConfiguration { TableServiceOnlyEnabled = true })
      {
      }
    }

    [UsedImplicitly]
    public sealed class AzuriteWithCustomContainerPortsFixture : AzuriteDefaultFixture
    {
      public AzuriteWithCustomContainerPortsFixture()
        : base(new AzuriteTestcontainerConfiguration
        {
          BlobContainerPort = 65501,
          QueueContainerPort = 65502,
          TableContainerPort = 65503,
        })
      {
      }
    }

    [UsedImplicitly]
    public sealed class AzuriteWithCustomWorkspaceFixture : AzuriteDefaultFixture, IDisposable
    {
      public AzuriteWithCustomWorkspaceFixture()
        : base(new AzuriteTestcontainerConfiguration
        {
          Location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D")),
        })
      {
        if (Configuration.Location != null)
        {
          Directory.CreateDirectory(Configuration.Location);
        }
      }

      public void Dispose()
      {
        if (Directory.Exists(Configuration.Location))
        {
          Directory.Delete(Configuration.Location, true);
        }
      }
    }
  }
}
