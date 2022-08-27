namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using RabbitMQ.Client;

  [UsedImplicitly]
  public sealed class RabbitMqFixture : DatabaseFixture<RabbitMqTestcontainer, IConnection>
  {
    private readonly TestcontainerMessageBrokerConfiguration configuration = new RabbitMqTestcontainerConfiguration { Username = "rabbitmq", Password = "rabbitmq" };

    public RabbitMqFixture()
    {
      this.Container = new TestcontainersBuilder<RabbitMqTestcontainer>()
        .WithMessageBroker(this.configuration)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      var connectionFactory = new ConnectionFactory();
      connectionFactory.Uri = new Uri(this.Container.ConnectionString);
      this.Connection = connectionFactory.CreateConnection();
    }

    public override async Task DisposeAsync()
    {
      this.Connection.Dispose();

      await this.Container.DisposeAsync()
        .ConfigureAwait(false);
    }

    public override void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
