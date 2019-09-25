namespace DotNet.Testcontainers.Tests.Unit
{
  using Core.Models;
  using Core.Wait;
  using Xunit;

  public class TestcontainersAccessNotPreConfiguredTest
  {
    private class NotPreConfigured : TestcontainerConfiguration
    {
      public NotPreConfigured() : base(string.Empty, 0)
      {
      }
    }

    [Fact]
    public void PreConfiguredWithoutTestcontainersImplementation()
    {
      // Given
      var notPreConfigured = new NotPreConfigured();

      // When
      // Then
      Assert.Equal(string.Empty, notPreConfigured.Image);
      Assert.Equal(0, notPreConfigured.DefaultPort);
      Assert.Equal(0, notPreConfigured.Port);
      Assert.Equal(0, notPreConfigured.Environments.Count);
      Assert.Equal("localhost", notPreConfigured.Hostname);
      Assert.Null(notPreConfigured.Username);
      Assert.Null(notPreConfigured.Password);
      Assert.Null(notPreConfigured.OutputConsumer);
      Assert.IsAssignableFrom<IWaitUntil>(notPreConfigured.WaitStrategy);
    }
  }
}
