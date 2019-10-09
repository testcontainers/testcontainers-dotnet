namespace DotNet.Testcontainers.Tests.Unit.Services
{
  using DotNet.Testcontainers.Services;
  using Xunit;

  public static class TestcontainersRegistryServiceTest
  {
    public class RegisterContainer
    {
      [Fact]
      public void RunTestcontainerWithCleanUp()
      {
        // Given
        const string id = nameof(this.RunTestcontainerWithCleanUp);

        // When
        TestcontainersRegistryService.Register(id, true);

        // Then
        Assert.Contains(id, TestcontainersRegistryService.GetRegisteredContainers());
      }

      [Fact]
      public void RunTestcontainerWithoutCleanUp()
      {
        // Given
        const string id = nameof(this.RunTestcontainerWithoutCleanUp);

        // When
        TestcontainersRegistryService.Register(id);

        // Then
        Assert.DoesNotContain(id, TestcontainersRegistryService.GetRegisteredContainers());
      }
    }

    public class UnregisterContainer
    {
      [Fact]
      public void RemoveTestcontainer()
      {
        // Given
        const string id = nameof(this.RemoveTestcontainer);

        // When
        TestcontainersRegistryService.Register(id, true);
        TestcontainersRegistryService.Unregister(id);

        // Then
        Assert.DoesNotContain(id, TestcontainersRegistryService.GetRegisteredContainers());
      }
    }
  }
}
