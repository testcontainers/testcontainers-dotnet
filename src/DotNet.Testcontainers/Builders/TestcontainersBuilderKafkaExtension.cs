namespace DotNet.Testcontainers.Builders
{
  using System.Linq;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// This class applies the extended Testcontainer configurations for Kafka.
  /// </summary>
  [PublicAPI]
  public static class TestcontainersBuilderKafkaExtension
  {
    public static ITestcontainersBuilder<KafkaTestcontainer> WithKafka(this ITestcontainersBuilder<KafkaTestcontainer> builder, KafkaTestcontainerConfiguration configuration)
    {
      builder = configuration.Environments.Aggregate(builder, (current, environment)
        => current.WithEnvironment(environment.Key, environment.Value));

      return builder
        .WithImage(configuration.Image)
        .WithCommand(configuration.Command)
        .WithPortBinding(configuration.Port, configuration.DefaultPort)
        .WithWaitStrategy(configuration.WaitStrategy)
        .WithStartupCallback(configuration.StartupCallback)
        .ConfigureContainer(container =>
        {
          container.ContainerPort = configuration.DefaultPort;
        });
    }
  }
}
