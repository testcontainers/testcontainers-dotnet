namespace Testcontainers.Temporal;

public sealed class TemporalContainerTest : IAsyncLifetime
{
    private readonly TemporalContainer _temporalContainer = new TemporalBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _temporalContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _temporalContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListNamespacesReturnsDefaultNamespace()
    {
        // Given
        var clientOptions = new TemporalClientConnectOptions();
        clientOptions.TargetHost = _temporalContainer.GetGrpcAddress();

        var connectedClient = await TemporalClient.ConnectAsync(clientOptions)
            .ConfigureAwait(true);

        // When
        var response = await connectedClient.WorkflowService.ListNamespacesAsync(new ListNamespacesRequest())
            .ConfigureAwait(true);

        // Then
        Assert.Contains(response.Namespaces, ns => ns.NamespaceInfo.Name == clientOptions.Namespace);
        Assert.Equal(_temporalContainer.GetGrpcAddress(), _temporalContainer.GetConnectionString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DescribeWorkflowReturnsStartedWorkflow()
    {
        // Given
        const string workflowType = "my-workflow";

        var workflowOptions = new WorkflowOptions();
        workflowOptions.Id = Guid.NewGuid().ToString("D");
        workflowOptions.TaskQueue = Guid.NewGuid().ToString("D");
        workflowOptions.Memo = new Dictionary<string, object> { { "env", "test" } };

        var clientOptions = new TemporalClientConnectOptions();
        clientOptions.TargetHost = _temporalContainer.GetGrpcAddress();

        var connectedClient = await TemporalClient.ConnectAsync(clientOptions)
            .ConfigureAwait(true);

        // When
        var runningWorkflow = await connectedClient.StartWorkflowAsync(workflowType, Array.Empty<object>(), workflowOptions)
            .ConfigureAwait(true);

        var workflowDescription = await runningWorkflow.DescribeAsync()
            .ConfigureAwait(true);

        // Then
        Assert.Equal(workflowType, workflowDescription.WorkflowType);
        Assert.Equal(workflowOptions.Id, workflowDescription.Id);
        Assert.Equal(workflowOptions.TaskQueue, workflowDescription.TaskQueue);
        Assert.True(workflowDescription.Memo.ContainsKey("env"));
    }
}