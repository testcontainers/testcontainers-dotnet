namespace DotNet.Testcontainers.Containers
{
  using System.Net.Http;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;
  using JetBrains.Annotations;

  public sealed class CosmosDbTestcontainer : TestcontainerDatabase
  {
    [PublicAPI]
    public int SqlApiPort
      => this.GetMappedPublicPort(this.ContainerSqlApiPort);

    [PublicAPI]
    public int MongoApiPort
      => this.GetMappedPublicPort(this.ContainerMongoApiPort);

    [PublicAPI]
    public int ContainerSqlApiPort { get; set; }

    [PublicAPI]
    public int ContainerMongoApiPort { get; set; }

    public string AccountEndpoint { get; }

    private HttpClient HttpClient;

    internal CosmosDbTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    public override string ConnectionString => 
      $"AccountEndpoint=https://{this.Hostname}:{this.SqlApiPort};AccountKey={this.Password}";
  }
}
