namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using RabbitMQ.Client;

  public sealed class RabbitMqFixture : ModuleFixture<RabbitMqTestcontainer>
  {
    public RabbitMqFixture()
      : base(new TestcontainersBuilder<RabbitMqTestcontainer>()
        .WithMessageBroker(new RabbitMqTestcontainerConfiguration
        {
          Username = "rabbitmq",
          Password = "rabbitmq",
        })
        .Build())
    {
    }

    public Task<IConnection> GetConnection()
    {
      var connectionFactory = new ConnectionFactory();
      connectionFactory.Uri = new Uri(this.Container.ConnectionString);
      return Task.FromResult(connectionFactory.CreateConnection());
    }

    public override Task InitializeAsync()
    {
      return this.Container.StartAsync();
    }

    public override Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
