namespace DotNet.Testcontainers.Tests.Unit.Windows
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers;
  using Xunit;

  public class TestcontainersContainerTest
  {
    public class With
    {
      [IgnoreOnLinuxEngine]
      public void IsWindowsEngineEnabled()
      {
        Assert.True(DockerHostConfiguration.IsWindowsEngineEnabled);
      }

      [IgnoreOnLinuxEngine]
      public async Task Disposable()
      {
        // Given
        // When
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/windows/nanoserver:1809");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
        }
      }
    }
  }
}
