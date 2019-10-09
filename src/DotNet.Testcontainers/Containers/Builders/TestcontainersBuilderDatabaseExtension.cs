namespace DotNet.Testcontainers.Containers.Builders
{
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
      foreach (var environment in configuration.Environments)
      {
        builder = builder.WithEnvironment(environment.Key, environment.Value);
      }

      return builder
        .WithImage(configuration.Image)
        .WithPortBinding(configuration.Port, configuration.DefaultPort)
        .WithOutputConsumer(configuration.OutputConsumer)
        .WithWaitStrategy(configuration.WaitStrategy)
        .ConfigureContainer(container =>
        {
          container.Hostname = configuration.Hostname;
          container.Port = configuration.Port;
          container.Database = configuration.Database;
          container.Username = configuration.Username;
          container.Password = configuration.Password;
        });
    }
  }
}
