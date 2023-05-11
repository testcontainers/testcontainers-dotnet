using Google.Api.Gax;
using Google.Cloud.Spanner.Admin.Instance.V1;


namespace Testcontainers.Spanner.Tests;

public class SpannerContainerTestsForEnvironmentVariable
{
    private const string InstanceId = "my-pretty-test-instance";
    private const string ProjectUri = "projects/my-project";



    [Fact]
    public async Task WhenProductionOnlyConnectionNotUsable()
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

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await client.CreateInstanceAsync(new CreateInstanceRequest()
            {
                InstanceId = InstanceId,
                Parent = ProjectUri,
            }));
        }
    }

    [Fact]
    public async Task WhenNotSetThenConnectionIsNotUseable()
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

            Environment.SetEnvironmentVariable("SPANNER_EMULATOR_HOST", null);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await client.CreateInstanceAsync(new CreateInstanceRequest()
            {
                InstanceId = InstanceId,
                Parent = ProjectUri,
            }));
        }
    }
     [Fact]
    public async Task WhenNotSetAndEmulatorOnlyThenConnectionIsNotUseable()
    {
        // Arrange
        var builder = new SpannerBuilder();
        await using (var containerManager = builder.Build())
        {
            // Act
            await containerManager.StartAsync();

            var clientBuilder = new InstanceAdminClientBuilder()
            {
                EmulatorDetection = EmulatorDetection.EmulatorOnly,
                Endpoint = $"{containerManager.Hostname}:{containerManager.GrpcPort}",
            };

            var client = await clientBuilder.BuildAsync();

            Environment.SetEnvironmentVariable("SPANNER_EMULATOR_HOST", null);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await client.CreateInstanceAsync(new CreateInstanceRequest()
            {
                InstanceId = InstanceId,
                Parent = ProjectUri,
            }));
        }
    }
}
