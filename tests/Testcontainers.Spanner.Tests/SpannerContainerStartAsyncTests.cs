using DotNet.Testcontainers.Containers;
using Google.Api.Gax;
using Google.Cloud.Spanner.Admin.Instance.V1;


namespace Testcontainers.Spanner.Tests;

public class SpannerContainerStartAsyncTests
{
    private const string InstanceId = "my-pretty-test-instance";
    private const string ProjectUri = "projects/my-project";

    [Fact]
    public async Task WhenCompleteThenContainerIsRunning()
    {
        // Arrange
        var builder = new SpannerBuilder();
        await using (var containerManager = builder.Build())
        {

            // Act
            await containerManager.StartAsync();
            // Assert

            Assert.Equal(TestcontainersStates.Running, containerManager.State);
        }
    }

    [Fact]
    public async Task WhenCompleteThenContainerHasRandomizedRestPortMapped()
    {
        // Arrange
        var builder = new SpannerBuilder();
        await using (var containerManager = builder.Build())
        {
            // Act
            await containerManager.StartAsync();
            // Assert

            Assert.NotEqual(default, containerManager.RestPort);
            Assert.NotEqual(9020, containerManager.RestPort);
        }
    }

    [Fact]
    public async Task WhenCompleteThenContainerHasRandomizedGrpcPortMapped()
    {
        // Arrange
        var builder = new SpannerBuilder();
        await using (var containerManager = builder.Build())
        {
            // Act
            await containerManager.StartAsync();

            // Assert
            Assert.NotEqual(default, containerManager.GrpcPort);
            Assert.NotEqual(9010, containerManager.GrpcPort);
        }
    }


    [Fact]
    public async Task WhenCompleteThenEnvironmentVariableEmulatorHostSet()
    {
        // Arrange
        var builder = new SpannerBuilder();
        await using (var containerManager = builder.Build())
        {
            // Act
            await containerManager.StartAsync();

            // Assert
            Assert.Equal($"{containerManager.Hostname}:{containerManager.GrpcPort}", Environment.GetEnvironmentVariable("SPANNER_EMULATOR_HOST"));
        }
    }

    [Fact]
    public async Task WhenCompleteThenConnectionIsUseable()
    {
        // Arrange
        var builder = new SpannerBuilder();
        await using (var containerManager = builder.Build())
        {
            // Act
            await containerManager.StartAsync();

            var clientBuilder = new InstanceAdminClientBuilder()
            {
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
                Endpoint = $"{containerManager.Hostname}:{containerManager.GrpcPort}",
            };

            var client = await clientBuilder.BuildAsync();

            // Act
            await client.CreateInstanceAsync(new CreateInstanceRequest()
            {
                InstanceId = InstanceId,
                Parent = ProjectUri,
            });

            var instances = client.ListInstances(new ListInstancesRequest
            {
                Parent = ProjectUri,
            });

            // Assert
            Assert.Contains(instances, i => i.InstanceName.InstanceId == InstanceId);
        }
    }
}
