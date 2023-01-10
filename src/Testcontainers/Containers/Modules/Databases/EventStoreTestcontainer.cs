namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerDatabase" />
  [PublicAPI]
  public sealed class EventStoreTestcontainer : TestcontainerDatabase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal EventStoreTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <inheritdoc />
    public override string ConnectionString
      => string.IsNullOrEmpty(this.Username) && string.IsNullOrEmpty(this.Password)
        ? $"esdb://{this.Hostname}:{this.Port}?tls=false"
        : $"esdb://{this.Username}:{this.Password}@{this.Hostname}:{this.Port}?tls=false";
  }
}
