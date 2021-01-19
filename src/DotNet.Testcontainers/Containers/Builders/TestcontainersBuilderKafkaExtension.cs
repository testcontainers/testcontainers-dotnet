namespace DotNet.Testcontainers.Containers.Builders
{
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Configurations.MessageBrokers;
  using DotNet.Testcontainers.Containers.Modules.MessageBrokers;

  /// <summary>
  /// This class applies the extended Testcontainer configurations for Kafka.
  /// </summary>
  public static class TestcontainersBuilderKafkaExtension
  {
    public static ITestcontainersBuilder<KafkaTestcontainer> WithKafka(this ITestcontainersBuilder<KafkaTestcontainer> builder, KafkaTestcontainerConfiguration configuration)
    {
      builder = configuration.Environments.Aggregate(builder, (current, environment) => current.WithEnvironment(environment.Key, environment.Value));

      return builder
        .WithImage(configuration.Image)
        .WithCommand(configuration.Command)
        .WithPortBinding(configuration.Port, configuration.DefaultPort)
        .WithWaitStrategy(configuration.WaitStrategy)
        .WithStartupCallback(configuration.StartupCallback)
        .ConfigureContainer(container =>
        {
          container.Port = configuration.Port;
        });
    }
  }
}
