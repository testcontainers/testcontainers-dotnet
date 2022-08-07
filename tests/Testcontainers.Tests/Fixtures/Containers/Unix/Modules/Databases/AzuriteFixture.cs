namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public static class AzuriteFixture
  {
    // We cannot use `Path.GetTempPath()` on macOS, see: https://github.com/common-workflow-language/cwltool/issues/328.
    private static readonly string TempDir = Environment.GetEnvironmentVariable("AGENT_TEMPDIRECTORY") ?? Directory.GetCurrentDirectory();

    public class AzuriteDefaultFixture : IAsyncLifetime
    {
      public AzuriteTestcontainer Container { get; private set; }

      public virtual Task InitializeAsync()
      {
        this.Container = new TestcontainersBuilder<AzuriteTestcontainer>()
          .WithAzurite(this.GetConfiguration())
          .Build();

        return this.Container.StartAsync();
      }

      public Task DisposeAsync()
      {
        if (this.Container == null)
        {
          return Task.CompletedTask;
        }

        return this.Container.DisposeAsync().AsTask();
      }

      protected virtual AzuriteTestcontainerConfiguration GetConfiguration()
      {
        return new AzuriteTestcontainerConfiguration();
      }
    }

    public sealed class AzuriteWithBlobOnlyFixture : AzuriteDefaultFixture
    {
      protected override AzuriteTestcontainerConfiguration GetConfiguration()
      {
        return new AzuriteTestcontainerConfiguration() { BlobServiceOnlyEnabled = true };
      }
    }

    public sealed class AzuriteWithQueueOnlyFixture : AzuriteDefaultFixture
    {
      protected override AzuriteTestcontainerConfiguration GetConfiguration()
      {
        return new AzuriteTestcontainerConfiguration() { QueueServiceOnlyEnabled = true };
      }
    }

    public sealed class AzuriteWithTableOnlyFixture : AzuriteDefaultFixture
    {
      protected override AzuriteTestcontainerConfiguration GetConfiguration()
      {
        return new AzuriteTestcontainerConfiguration() { TableServiceOnlyEnabled = true };
      }
    }

    public sealed class AzuriteWithCustomContainerPortsFixture : AzuriteDefaultFixture
    {
      public int CustomBlobContainerPort { get; } = 7777;

      public int CustomQueueContainerPort { get; } = 8888;

      public int CustomTableContainerPort { get; } = 9999;

      protected override AzuriteTestcontainerConfiguration GetConfiguration()
      {
        return new AzuriteTestcontainerConfiguration()
        {
          BlobContainerPort = this.CustomBlobContainerPort,
          QueueContainerPort = this.CustomQueueContainerPort,
          TableContainerPort = this.CustomTableContainerPort,
        };
      }
    }

    public sealed class AzuriteWithBoundLocationPathFixture : AzuriteDefaultFixture, IDisposable
    {
      public DirectoryInfo DataDirectoryPath { get; } = new DirectoryInfo(Path.Combine(TempDir, Guid.NewGuid().ToString("N")));

      public override Task InitializeAsync()
      {
        this.DataDirectoryPath.Create();
        return base.InitializeAsync();
      }

      public void Dispose()
      {
        this.DataDirectoryPath.Delete(true);
      }

      protected override AzuriteTestcontainerConfiguration GetConfiguration()
      {
        return new AzuriteTestcontainerConfiguration()
        {
          Location = this.DataDirectoryPath.FullName,
        };
      }
    }
  }
}
