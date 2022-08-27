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
    // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328.
    private static readonly string TempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? Directory.GetCurrentDirectory();

    [UsedImplicitly]
    public class AzuriteDefaultFixture : IAsyncLifetime
    {
      public AzuriteDefaultFixture()
        : this(new AzuriteTestcontainerConfiguration())
      {
      }

      protected AzuriteDefaultFixture(AzuriteTestcontainerConfiguration configuration)
      {
        this.Configuration = configuration;
        this.Container = new TestcontainersBuilder<AzuriteTestcontainer>()
          .WithAzurite(configuration)
          .Build();
      }

      public AzuriteTestcontainerConfiguration Configuration { get; }

      public AzuriteTestcontainer Container { get; }

      public Task InitializeAsync()
      {
        return this.Container.StartAsync();
      }

      public Task DisposeAsync()
      {
        return this.Container.DisposeAsync().AsTask();
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
          Location = Path.Combine(TempDir, Guid.NewGuid().ToString("N")),
        })
      {
        if (this.Configuration.Location != null)
        {
          Directory.CreateDirectory(this.Configuration.Location);
        }
      }

      public void Dispose()
      {
        if (Directory.Exists(this.Configuration.Location))
        {
          Directory.Delete(this.Configuration.Location, true);
        }
      }
    }
  }
}
