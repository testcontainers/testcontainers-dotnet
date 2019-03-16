namespace DotNet.Testcontainers.Core.Builder
{
  using DotNet.Testcontainers.Core.Containers.Database;
  using DotNet.Testcontainers.Core.Models.Database;
  using DotNet.Testcontainers.Core.Wait;

  public static class TestcontainersBuilderDatabaseExtension
  {
    public static ITestcontainersBuilder<T> WithDatabase<T>(this ITestcontainersBuilder<T> builder, DatabaseConfiguration configuration)
      where T : DatabaseContainer
    {
      foreach (var environment in configuration.Environments)
      {
        builder = builder.WithEnvironment(environment.Key, environment.Value);
      }

      return builder
        .WithImage(configuration.Image)
        .WithPortBinding(configuration.Port, configuration.DefaultPort)
        .WithWaitStrategy(new WaitUntilPortIsAvailable(configuration.Port))
        .ConfigureContainer((container) =>
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
