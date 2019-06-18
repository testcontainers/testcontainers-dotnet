namespace DotNet.Testcontainers.Core.Builder
{
  using DotNet.Testcontainers.Core.Containers;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Core.Wait;

  public static class TestcontainersBuilderMessageBrokerExtension
  {
    public static ITestcontainersBuilder<T> WithMessageBroker<T>(this ITestcontainersBuilder<T> builder, TestcontainerMessageBrokerConfiguration configuration)
      where T : TestcontainerMessageBroker
    {
      foreach (var environment in configuration.Environments)
      {
        builder = builder.WithEnvironment(environment.Key, environment.Value);
      }

      return builder
        .WithImage(configuration.Image)
        .WithPortBinding(configuration.Port, configuration.DefaultPort)
        .WithWaitStrategy(Wait.UntilPortsAreAvailable(configuration.DefaultPort))
        .ConfigureContainer((container) =>
        {
          container.Hostname = configuration.Hostname;
          container.Port = configuration.Port;
          container.Username = configuration.Username;
          container.Password = configuration.Password;
        });
    }
  }
}
