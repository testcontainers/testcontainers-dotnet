namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers.Database;
  using Xunit;

  public class DatabaseContainerTest
  {
    [Fact]
    public async Task PostgreSqlContainer()
    {
      var database = string.Empty;

      var username = string.Empty;

      var password = string.Empty;

      var testcontainersBuilder = new TestcontainersBuilder<PostgreSqlContainer>()
        .WithDatabase(database, username, password);

      using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();
      }
    }
  }
}
