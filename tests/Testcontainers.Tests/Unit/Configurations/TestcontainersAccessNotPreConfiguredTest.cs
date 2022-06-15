namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Linq;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public sealed class TestcontainersAccessNotPreConfiguredTest
  {
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
      Assert.IsAssignableFrom<RedirectStdoutAndStderrToNull>(notPreConfigured.OutputConsumer);
      Assert.IsAssignableFrom<UntilContainerIsRunning>(notPreConfigured.WaitStrategy.Build().First());
    }

    private sealed class NotPreConfigured : HostedServiceConfiguration
    {
      public NotPreConfigured()
        : base(string.Empty, 0)
      {
      }
    }
  }
}
