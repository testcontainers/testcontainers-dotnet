namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerDatabase" />
  [PublicAPI]
  public sealed class CouchDbTestcontainer : TestcontainerDatabase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal CouchDbTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <inheritdoc />
    /// <remarks>
    /// Do not use clear-text protocol in production, override it.
    /// </remarks>

    // https://github.com/SonarSource/sonar-dotnet/issues/4724
#pragma warning disable S5332

    public override string ConnectionString
      => $"http://{this.Username}:{this.Password}@{this.Hostname}:{this.Port}";

#pragma warning restore S5332
  }
}
