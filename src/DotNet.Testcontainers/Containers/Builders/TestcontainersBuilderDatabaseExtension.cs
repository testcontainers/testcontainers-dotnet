namespace DotNet.Testcontainers.Containers.Builders
{
  using System.Linq;
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.Modules.Abstractions;

  /// <summary>
  /// This class applies the extended Testcontainer configurations for databases.
  /// </summary>
  public static class TestcontainersBuilderDatabaseExtension
  {
    public static ITestcontainersBuilder<T> WithDatabase<T>(this ITestcontainersBuilder<T> builder, TestcontainerDatabaseConfiguration configuration)
      where T : TestcontainerDatabase
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
          container.Database = configuration.Database;
          container.Username = configuration.Username;
          container.Password = configuration.Password;
        });
    }
  }
}
