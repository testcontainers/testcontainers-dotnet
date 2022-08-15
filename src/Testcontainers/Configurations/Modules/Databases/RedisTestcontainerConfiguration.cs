namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class RedisTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string RedisImage = "redis:5.0.14";

    private const int RedisPort = 6379;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisTestcontainerConfiguration" /> class.
    /// </summary>
    public RedisTestcontainerConfiguration()
      : this(RedisImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public RedisTestcontainerConfiguration(string image)
      : base(image, RedisPort)
    {
    }

    /// <inheritdoc />
    public override string Database
    {
      get => string.Empty;
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string Username
    {
      get => string.Empty;
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string Password
    {
      get => string.Empty;
      set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted("redis-cli", "ping");
  }
}
