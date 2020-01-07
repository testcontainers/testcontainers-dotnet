namespace DotNet.Testcontainers.Containers.Configurations.Databases
{
  using System;
  using DotNet.Testcontainers.Containers.Configurations.Abstractions;
  using DotNet.Testcontainers.Containers.WaitStrategies;

  public sealed class RedisTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public RedisTestcontainerConfiguration() : base("redis:5.0.6", 6379)
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

    public override IWaitUntil WaitStrategy => new WaitUntilShellCommandsAreCompleted("redis-cli ping");
  }
}
