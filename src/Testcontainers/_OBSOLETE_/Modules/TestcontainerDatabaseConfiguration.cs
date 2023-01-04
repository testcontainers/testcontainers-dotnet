namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents an extended Testcontainer configuration for databases.
  /// </summary>
  public abstract class TestcontainerDatabaseConfiguration : HostedServiceConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainerDatabaseConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <param name="defaultPort">The container port.</param>
    /// <param name="port">The host port.</param>
    protected TestcontainerDatabaseConfiguration(string image, int defaultPort, int port = 0)
      : base(image, defaultPort, port)
    {
    }

    /// <summary>
    /// Gets or sets the database.
    /// </summary>
    [PublicAPI]
    public virtual string Database { get; set; }
  }
}
