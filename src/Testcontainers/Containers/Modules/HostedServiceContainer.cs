namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// This class represents an extended configured and created Testcontainer. It is convenient for common Testcontainers to provide a module with all necessary properties, which does not exist in <see cref="TestcontainersContainer" />.
  /// </summary>
  public abstract class HostedServiceContainer : TestcontainersContainer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HostedServiceContainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    protected HostedServiceContainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the host port.
    /// </summary>
    [PublicAPI]
    public int Port => this.GetMappedPublicPort(this.ContainerPort);

    /// <summary>
    /// Gets or sets the container port.
    /// </summary>
    [PublicAPI]
    public virtual int ContainerPort { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    [PublicAPI]
    public virtual string Username { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [PublicAPI]
    public virtual string Password { get; set; }
  }
}
