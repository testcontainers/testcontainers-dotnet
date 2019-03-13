namespace DotNet.Testcontainers.Core.Builder
{
  using DotNet.Testcontainers.Core.Containers.Database;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Core.Wait;

  public static class TestcontainersBuilderPostgreSqlExtension
  {
    private const int DefaultPort = 5432;

    private const string DefaultImage = "postgres:11.2";

    private const string Database = "POSTGRES_DB";

    private const string Username = "POSTGRES_USER";

    private const string Password = "POSTGRES_PASSWORD";

    public static ITestcontainersBuilder<T> WithDatabase<T>(this ITestcontainersBuilder<T> builder, DatabaseConfiguration configuration)
        where T : PostgreSqlContainer
    {
      configuration.Port = configuration.Port > 0 ? configuration.Port : DefaultPort;

      return builder
        .WithImage(DefaultImage)
        .WithPortBinding(configuration.Port)
        .WithEnvironment(Database, configuration.Database)
        .WithEnvironment(Username, configuration.Username)
        .WithEnvironment(Password, configuration.Password)
        .WithWaitStrategy(new WaitUntilPortIsAvailable(DefaultPort))
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
