namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class RedisTestcontainerTest : IClassFixture<RedisFixture>
  {
    private readonly RedisFixture redisFixture;

    public RedisTestcontainerTest(RedisFixture redisFixture)
    {
      this.redisFixture = redisFixture;
    }

    [Fact]
    public void ConnectionEstablished()
    {
      Assert.True(this.redisFixture.Connection.IsConnected);
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
