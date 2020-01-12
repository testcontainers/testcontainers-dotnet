namespace DotNet.Testcontainers.Tests.Unit.Containers.Windows
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using Xunit;

  public static class TestcontainersContainerTest
  {
    public class With
    {
      [IgnoreOnLinuxEngine]
      public async Task IsWindowsEngineEnabled()
      {
        Assert.True(await new TestcontainersClient().GetIsWindowsEngineEnabled());
      }

      [IgnoreOnLinuxEngine]
      public async Task Disposable()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/windows/nanoserver:1809");

        // When
        // Then
        using (IDockerContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }
    }
  }
}
