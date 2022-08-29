namespace DotNet.Testcontainers.Builders
{
  using System.Linq;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// This class applies the extended Testcontainer configurations for message brokers.
  /// </summary>
  [PublicAPI]
  public static class TestcontainersBuilderMessageBrokerExtension
  {
    public static ITestcontainersBuilder<T> WithMessageBroker<T>(this ITestcontainersBuilder<T> builder, TestcontainerMessageBrokerConfiguration configuration)
      where T : TestcontainerMessageBroker
    {
      builder = configuration.Environments.Aggregate(builder, (current, environment)
        => current.WithEnvironment(environment.Key, environment.Value));

      return builder
        .WithImage(configuration.Image)
        .WithExposedPort(configuration.DefaultPort)
        .WithPortBinding(configuration.Port, configuration.DefaultPort)
        .WithOutputConsumer(configuration.OutputConsumer)
        .WithWaitStrategy(configuration.WaitStrategy)
        .ConfigureContainer(container =>
        {
          container.ContainerPort = configuration.DefaultPort;
          container.Username = configuration.Username;
          container.Password = configuration.Password;
        });
    }
  }
}
