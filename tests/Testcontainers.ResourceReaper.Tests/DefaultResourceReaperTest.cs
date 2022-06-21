namespace DotNet.Testcontainers.ResourceReaper.Tests
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Unit;
  using Xunit;

  // NOTICE: These tests stop the static shared default Resource Reaper (dispose). Unit tests are executed parallel.
  // We cannot stop the default Resource Reaper all of a sudden. Run these tests in an isolated assembly.
  public sealed class DefaultResourceReaperTest : IAsyncLifetime
  {
    private static readonly string DefaultRyukContainerName = $"testcontainers-ryuk-{ResourceReaper.DefaultSessionId:D}";

    [Fact]
    public async Task CleanUpFalseDoesNotStartDefaultResourceReaper()
    {
      // Given
      var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("nginx")
        .WithAutoRemove(true)
        .WithCleanUp(false);

      // When
      await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync()
          .ConfigureAwait(false);
      }

      // Then
      Assert.False(Docker.Exists(DockerResource.Container, DefaultRyukContainerName));
    }

    [Fact]
    public async Task CleanUpTrueDoesStartDefaultResourceReaper()
    {
      // Given
      var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("nginx")
        .WithCleanUp(true);

      // When
      await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync()
          .ConfigureAwait(false);
      }

      // Then
      Assert.True(Docker.Exists(DockerResource.Container, DefaultRyukContainerName));
    }

    [Fact]
    public async Task UsingResourceReaperSessionIdDoesNotStartDefaultResourceReaper()
    {
      // Given
      var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("nginx")
        .WithAutoRemove(true)
        .WithResourceReaperSessionId(Guid.NewGuid());

      // When
      await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync()
          .ConfigureAwait(false);
      }

      // Then
      Assert.False(Docker.Exists(DockerResource.Container, DefaultRyukContainerName));
    }

    public async Task InitializeAsync()
    {
      if (Docker.Exists(DockerResource.Container, DefaultRyukContainerName))
      {
        var resourceReaper = await ResourceReaper.GetAndStartDefaultAsync()
          .ConfigureAwait(false);
        await resourceReaper.DisposeAsync()
          .ConfigureAwait(false);
      }
    }

    public Task DisposeAsync()
    {
      return Task.CompletedTask;
    }
  }
}
