namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
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

    [Fact]
    public async Task ExecScriptInRunningContainer()
    {
      // Given
      const string script = @"
        -- Lua script
        for i = 1, 5, 1 do
          redis.call('incr', 'my-counter')
        end
        local mycounter = redis.call('get', 'my-counter')
        return mycounter
      ";

      // When
      var result = await this.redisFixture.Container.ExecScriptAsync(script)
        .ConfigureAwait(false);

      // Then
      Assert.Equal(0, result.ExitCode);
      Assert.Contains("5", result.Stdout);
    }
  }
}
