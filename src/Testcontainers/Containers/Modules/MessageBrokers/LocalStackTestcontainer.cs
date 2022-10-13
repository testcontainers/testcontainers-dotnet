namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerMessageBroker" />
  [PublicAPI]
  public sealed class LocalStackTestcontainer : TestcontainerMessageBroker
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal LocalStackTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the LocalStack endpoint.
    /// </summary>
    public string ConnectionString
      => $"https://{this.Hostname}:{this.Port}";
  }
}
