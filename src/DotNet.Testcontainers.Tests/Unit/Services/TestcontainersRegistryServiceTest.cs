namespace DotNet.Testcontainers.Tests.Unit.Services
{
  using DotNet.Testcontainers.Services;
  using Xunit;

  public static class TestcontainersRegistryServiceTest
  {
    public class RegisterContainer
    {
      private readonly TestcontainersRegistryService registryService = new TestcontainersRegistryService();

      [Fact]
      public void RunTestcontainerWithCleanUp()
      {
        // Given
        const string id = nameof(this.RunTestcontainerWithCleanUp);

        // When
        this.registryService.Register(id, true);

        // Then
        Assert.Contains(id, this.registryService.GetRegisteredContainers());
      }

      [Fact]
      public void RunTestcontainerWithoutCleanUp()
      {
        // Given
        const string id = nameof(this.RunTestcontainerWithoutCleanUp);

        // When
        this.registryService.Register(id);

        // Then
        Assert.DoesNotContain(id, this.registryService.GetRegisteredContainers());
      }
    }

    public class UnregisterContainer
    {
      private readonly TestcontainersRegistryService registryService = new TestcontainersRegistryService();

      [Fact]
      public void RemoveTestcontainer()
      {
        // Given
        const string id = nameof(this.RemoveTestcontainer);

        // When
        this.registryService.Register(id, true);
        this.registryService.Unregister(id);

        // Then
        Assert.DoesNotContain(id, this.registryService.GetRegisteredContainers());
      }
    }
  }
}
