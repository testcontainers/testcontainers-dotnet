using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace DotNet.Testcontainers.Commons
{
    public abstract class ContainerTest<TBuilderEntity, TContainerEntity> : IAsyncLifetime
        where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
        where TContainerEntity : IContainer
    {
        protected ContainerTest() : this(null)
        {
        }

        protected ContainerTest(Action<TBuilderEntity> configure)
        {
            var builder = new TBuilderEntity();
            configure?.Invoke(builder);
            Container = builder.Build();
        }

        protected TContainerEntity Container { get; }

        public async Task InitializeAsync()
        {
            await Container.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
        }
    }
}