namespace Testcontainers.Kafka;

using DotNet.Testcontainers.Configurations;

public sealed class KafkaEntrypointTest
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PreservesEntrypointAndCommand(bool customEntrypoint, bool customCommand)
    {
        var builder = new KafkaBuilder("apache/kafka:4.0.2")
            .WithKRaft()
            .WithWaitStrategy(Wait.ForUnixContainer());

        if (customEntrypoint)
        {
            builder = builder.WithEntrypoint("/bin/bash", "-c");
        }

        if (customCommand)
        {
            builder = builder.WithCommand(new OverwriteEnumerable<string>(new[] { "exec sleep infinity" }));
        }

        await using var container = builder.Build();
        using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder(Guid.NewGuid()).Build();

        for (var start = 0; start < 2; start++)
        {
            await container.StartAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            var inspection = await dockerClient.Containers.InspectContainerAsync(container.Id, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            Assert.Equal(new[] { customEntrypoint ? "/bin/bash" : "/bin/sh", "-c" }, inspection.Config.Entrypoint);

            if (customCommand)
            {
                Assert.Equal(new[] { "exec sleep infinity" }, inspection.Config.Cmd);
            }
            else
            {
                var command = Assert.Single(inspection.Config.Cmd);
                Assert.Contains("while [ ! -s /testcontainers.ready ]", command);
                Assert.EndsWith("exec " + KafkaBuilder.StartupScriptFilePath, command);
            }

            var startupScript = await container.ReadFileAsync(KafkaBuilder.StartupScriptFilePath, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);
            Assert.Contains("exec /etc/kafka/docker/run", Encoding.Default.GetString(startupScript));

            var readyFile = await container.ReadFileAsync("/testcontainers.ready", TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            if (customCommand)
            {
                Assert.Equal(new byte[] { 1 }, readyFile);

                // The startup callback must replace both files on the next start.
                await container.CopyAsync(Encoding.Default.GetBytes("stale script"), KafkaBuilder.StartupScriptFilePath, ct: TestContext.Current.CancellationToken)
                    .ConfigureAwait(true);
                await container.CopyAsync(Array.Empty<byte>(), "/testcontainers.ready", ct: TestContext.Current.CancellationToken)
                    .ConfigureAwait(true);
            }

            await container.StopAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);
        }
    }
}
