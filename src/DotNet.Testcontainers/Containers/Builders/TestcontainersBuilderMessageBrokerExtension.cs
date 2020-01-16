namespace DotNet.Testcontainers.Containers.Builders
{
  using System.Linq;
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  /// <summary>
  /// This class applies the extended Testcontainer configurations for message brokers.
  /// </summary>
  public static class TestcontainersBuilderMessageBrokerExtension
  {
    public static ITestcontainersBuilder<T> WithMessageBroker<T>(this ITestcontainersBuilder<T> builder, TestcontainerMessageBrokerConfiguration configuration)
      where T : TestcontainerMessageBroker
    {
      builder = configuration.Environments.Aggregate(builder, (current, environment) => current.WithEnvironment(environment.Key, environment.Value));

      return builder
        .WithImage(configuration.Image)
        .WithPortBinding(configuration.Port, configuration.DefaultPort)
        .WithOutputConsumer(configuration.OutputConsumer)
        .WithWaitStrategy(configuration.WaitStrategy)
        .ConfigureContainer(container =>
        {
          container.Port = configuration.Port;
          container.Username = configuration.Username;
          container.Password = configuration.Password;
        });
    }
  }
}
