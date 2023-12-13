namespace Testcontainers.Tests;

using Testcontainers.PostgreSql;

public sealed class ConnectionStringProviderTest
{
    [Fact]
    public void Test()
    {
        IConnectionStringProvider c1 = new ContainerBuilder()
            .WithConnectionStringProvider(new MyProvider1())
            .Build();

        IConnectionStringProvider c2 = new PostgreSqlBuilder()
            .WithConnectionStringProvider(new MyProvider2())
            .Build();

        _ = c1.GetConnectionString();

        _ = c1.GetConnectionString(ConnectionMode.Host);

        _ = c1.GetConnectionString(ConnectionMode.Container);
    }

    private sealed class MyProvider1 : IConnectionStringProvider<IContainer, IContainerConfiguration>
    {
        public void Build(IContainer container, IContainerConfiguration configuration)
        {
            // TODO: Add connection string for host and container communication.
        }
    }
    
    private sealed class MyProvider2 : IConnectionStringProvider<PostgreSqlContainer, PostgreSqlConfiguration>
    {
        public void Build(PostgreSqlContainer container, PostgreSqlConfiguration configuration)
        {
            // TODO: Add connection string for host and container communication.
        }
    }
}