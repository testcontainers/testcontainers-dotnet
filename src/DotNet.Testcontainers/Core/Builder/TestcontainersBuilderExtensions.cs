namespace DotNet.Testcontainers.Core.Builder
{
  using DotNet.Testcontainers.Core.Containers.Database;

  public static class TestcontainersBuilderExtensions
  {
    public static ITestcontainersBuilder<T> WithDatabase<T>(this ITestcontainersBuilder<T> builder, string database, string username, string password)
        where T : PostgreSqlContainer
    {
      return builder
        .WithImage("postgres")
        .WithCommand("postgres")
        .WithExposedPort(5432)
        .WithEnvironment("POSTGRES_DB", database)
        .WithEnvironment("POSTGRES_USER", username)
        .WithEnvironment("POSTGRES_PASSWORD", password)
        .ConfigureContainer((container) =>
        {
          container.Database = database;
          container.Username = username;
          container.Password = password;
        });
    }
  }
}
