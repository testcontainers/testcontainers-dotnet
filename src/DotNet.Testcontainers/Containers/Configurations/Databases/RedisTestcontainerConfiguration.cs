namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using System;
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public class RedisTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string RedisImage = "redis:5.0.6";

    private const int RedisPort = 6379;

    public RedisTestcontainerConfiguration()
      : this(RedisImage)
    {
    }

    public RedisTestcontainerConfiguration(string image)
      : base(image, RedisPort)
    {
    }

    public override string Database
    {
      get => string.Empty;
      set => throw new NotImplementedException();
    }

    public override string Username
    {
      get => string.Empty;
      set => throw new NotImplementedException();
    }

    public override string Password
    {
      get => string.Empty;
      set => throw new NotImplementedException();
    }

    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted("redis-cli ping");
  }
}
