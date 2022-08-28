namespace DotNet.Testcontainers.Builders
{
    using DotNet.Testcontainers.Configurations;
    using DotNet.Testcontainers.Containers;
    using JetBrains.Annotations;

    [PublicAPI]
    public static class TestcontainersBuilderCosmosDbExtension
    {
        public static ITestcontainersBuilder<CosmosDbTestcontainer> WithCosmosDb(
            this ITestcontainersBuilder<CosmosDbTestcontainer> builder, CosmosDbTestcontainerConfiguration configuration)
        {
          builder.WithImage(configuration.Image)
              .WithWaitStrategy(configuration.WaitStrategy)
              .WithExposedPort(configuration.SqlApiContainerPort)
              .WithPortBinding(configuration.SqlApiPort);

            return builder;
        }
    }
}
