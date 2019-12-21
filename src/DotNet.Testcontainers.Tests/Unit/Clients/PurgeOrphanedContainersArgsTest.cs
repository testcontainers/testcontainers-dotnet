namespace DotNet.Testcontainers.Tests.Unit.Clients
{
  using System;
  using DotNet.Testcontainers.Clients;
  using Xunit;

  public class PurgeOrphanedContainersArgsTest
  {
    [Fact]
    public void PurgeLocalRegisteredContainers()
    {
      // Given
      var endpoint = DockerApiEndpoint.Local;

      var args = new PurgeOrphanedContainersArgs(endpoint, new [] { nameof(this.PurgeLocalRegisteredContainers) });

      // When
      var actual = args.ToString();

      // Then
      Assert.DoesNotContain("-H", actual);
      Assert.Contains($"rm --force {nameof(this.PurgeLocalRegisteredContainers)}", actual);
    }

    [Fact]
    public void PurgeRemoteRegisteredContainers()
    {
      // Given
      var endpoint = new Uri("tcp://127.0.0.1:2376");

      var args = new PurgeOrphanedContainersArgs(endpoint, new [] { nameof(this.PurgeRemoteRegisteredContainers) });

      // When
      var actual = args.ToString();

      // Then
      Assert.Contains($"-H {endpoint}", actual);
      Assert.Contains($"rm --force {nameof(this.PurgeRemoteRegisteredContainers)}", actual);
    }
  }
}
