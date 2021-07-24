namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// This class represents an extended configured Testcontainer for databases.
  /// </summary>
  public abstract class TestcontainerDatabase : HostedServiceContainer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainerDatabase" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    protected TestcontainerDatabase(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the database connection string.
    /// </summary>
    [PublicAPI]
    public abstract string ConnectionString { get; }

    /// <summary>
    /// Gets or sets the database.
    /// </summary>
    [PublicAPI]
    public virtual string Database { get; set; }
  }
}
