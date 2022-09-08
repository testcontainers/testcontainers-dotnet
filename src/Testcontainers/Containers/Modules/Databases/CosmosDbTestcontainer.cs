namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  public sealed class CosmosDbTestcontainer : TestcontainerDatabase
  {
    internal CosmosDbTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    public override string ConnectionString =>
      $"AccountEndpoint=https://{this.Hostname}:{this.Port};AccountKey={this.Password}";
  }
}
