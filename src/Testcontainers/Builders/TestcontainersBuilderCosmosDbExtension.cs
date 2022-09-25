namespace DotNet.Testcontainers.Builders
{
  using System.Linq;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  [PublicAPI]
  public static class TestcontainersBuilderCosmosDbExtension
  {
    public static ITestcontainersBuilder<CosmosDbTestcontainer> WithCosmosDb(
      this ITestcontainersBuilder<CosmosDbTestcontainer> builder, CosmosDbTestcontainerConfiguration configuration)
    {
      builder = configuration.Environments.Aggregate(builder, (current, environment)
        => current.WithEnvironment(environment.Key, environment.Value));

      return builder.WithImage(configuration.Image)
        .WithPortBinding(configuration.DefaultPort, true)
        .WithExposedPort(configuration.DefaultPort)
        .WithWaitStrategy(configuration.WaitStrategy)
        .WithOutputConsumer(configuration.OutputConsumer)
        .ConfigureContainer(testcontainer =>
        {
          testcontainer.ContainerPort = configuration.DefaultPort;
          testcontainer.Password = configuration.Password;
        });
    }
  }
}
