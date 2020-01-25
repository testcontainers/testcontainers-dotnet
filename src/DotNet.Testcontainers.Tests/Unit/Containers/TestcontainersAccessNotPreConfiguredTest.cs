namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System.Linq;
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.OutputConsumers.Common;
  using DotNet.Testcontainers.Containers.WaitStrategies.Common;
  using Xunit;

  public class TestcontainersAccessNotPreConfiguredTest
  {
    private class NotPreConfigured : HostedServiceConfiguration
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
      Assert.Null(notPreConfigured.Username);
      Assert.Null(notPreConfigured.Password);
      Assert.IsAssignableFrom<DoNotConsumeStdoutOrStderr>(notPreConfigured.OutputConsumer);
      Assert.IsAssignableFrom<UntilContainerIsRunning>(notPreConfigured.WaitStrategy.Build().First());
    }
  }
}
