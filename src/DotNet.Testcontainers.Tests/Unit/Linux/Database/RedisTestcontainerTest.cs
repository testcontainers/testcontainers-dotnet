namespace DotNet.Testcontainers.Tests.Unit.Linux.Database
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers.Database;
  using DotNet.Testcontainers.Core.Models.Database;
  using StackExchange.Redis;
  using Xunit;

  public class RedisTestcontainerTest
  {
    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var testcontainersBuilder = new TestcontainersBuilder<RedisTestcontainer>()
        .WithDatabase(new RedisTestcontainerConfiguration()); // TODO: Until now the configuration is not applied by `RedisTestcontainerConfiguration`. Use `WithCommand` or mount redis.conf instead.

      // When
      // Then
      using (var testcontainer = testcontainersBuilder.Build())
      {
        await testcontainer.StartAsync();

        using (var connection = ConnectionMultiplexer.Connect(testcontainer.ConnectionString))
        {
          Assert.True(connection.GetServer(testcontainer.Hostname, testcontainer.Port).Ping().Milliseconds > 0, "Cannot connect to Redis Testcontainer.");
        }
      }
    }

    [Fact]
    public void CannotSetDatabase()
    {
      var redis = new RedisTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => redis.Database = string.Empty);
    }

    [Fact]
    public void CannotSetUsername()
    {
      var redis = new RedisTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => redis.Username = string.Empty);
    }

    [Fact]
    public void CannotSetPassword()
    {
      var redis = new RedisTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => redis.Password = string.Empty);
    }
  }
}
