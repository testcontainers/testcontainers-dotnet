namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerDatabase" />
  [PublicAPI]
  public sealed class MySqlTestcontainer : TestcontainerDatabase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal MySqlTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <inheritdoc />
    public override string ConnectionString
      => $"Server={this.Hostname};Port={this.Port};Database={this.Database};Uid={this.Username};Pwd={this.Password};";
  }
}
