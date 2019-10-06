namespace DotNet.Testcontainers.Tests.Unit
{
  using Clients;
  using Xunit;

  public static class ContainerRegistryTest
  {
    public class RegisterContainer
    {
      [Fact]
      public void RunTestcontainerWithCleanUp()
      {
        // Given
        const string id = nameof(this.RunTestcontainerWithCleanUp);

        // When
        ContainerRegistry.Register(id, true);

        // Then
        Assert.Contains(id, ContainerRegistry.GetRegisteredContainers());
      }

      [Fact]
      public void RunTestcontainerWithoutCleanUp()
      {
        // Given
        const string id = nameof(this.RunTestcontainerWithoutCleanUp);

        // When
        ContainerRegistry.Register(id);

        // Then
        Assert.DoesNotContain(id, ContainerRegistry.GetRegisteredContainers());
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
        ContainerRegistry.Register(id, true);
        ContainerRegistry.Unregister(id);

        // Then
        Assert.DoesNotContain(id, ContainerRegistry.GetRegisteredContainers());
      }
    }
  }
}
