namespace DotNet.Testcontainers.Core.Models.Database
{
  using System;

  public sealed class RedisTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    public RedisTestcontainerConfiguration() : base("redis:5.0.5", 6379)
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
  }
}
