namespace Testcontainers.Tests;

public sealed class NetworkConnectTest
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConnectsRunningContainerToExistingNetworks()
    {
        // Given
        await using var networkByName = new NetworkBuilder()
            .Build();

        await using var networkByReference = new NetworkBuilder()
            .Build();

        await using var container = new ContainerBuilder(CommonImages.Alpine)
            .WithCommand(CommonCommands.SleepInfinity)
            .Build();

        await networkByName.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await networkByReference.CreateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await container.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder(Guid.NewGuid()).Build();

        // When
        await container.ConnectAsync(networkByName.Name, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await container.ConnectAsync(networkByReference, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var response = await dockerClient.Containers.InspectContainerAsync(container.Id, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Contains(networkByName.Name, response.NetworkSettings.Networks.Keys);
        Assert.Contains(networkByReference.Name, response.NetworkSettings.Networks.Keys);
    }
}