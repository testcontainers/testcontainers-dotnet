namespace Testcontainers.Temporalio;

public sealed class TemporalContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseTemporalContainer]
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
        var client = await TemporalClient.ConnectAsync(new(_temporalContainer.GetGrpcAddress())
        {
            Namespace = "default",
        }).ConfigureAwait(true);

        // When
        var response = await client.Connection.WorkflowService.ListNamespacesAsync(new()).ConfigureAwait(true);

        // Then
        Assert.Contains(response.Namespaces, ns => ns.NamespaceInfo.Name == "default");
        Assert.Equal(_temporalContainer.GetGrpcAddress(), _temporalContainer.GetConnectionString());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DescribeWorkflowReturnsStartedWorkflow()
    {
        // Given
        var client = await TemporalClient.ConnectAsync(new(_temporalContainer.GetGrpcAddress())
        {
            Namespace = "default",
        }).ConfigureAwait(true);

        var workflowId = Guid.NewGuid().ToString("D");

        // When
        var workflowHandle = await client.StartWorkflowAsync("MyWorkflow", Array.Empty<object>(), new(id: workflowId, taskQueue: "my-task-queue")
        {
            Memo = new Dictionary<string, object> { ["env"] = "test" },
        }).ConfigureAwait(true);

        var workflowExecutionDescription = await workflowHandle.DescribeAsync().ConfigureAwait(true);

        // Then
        Assert.Equal(workflowId, workflowExecutionDescription.Id);
        Assert.Equal("MyWorkflow", workflowExecutionDescription.WorkflowType);
        Assert.Equal("my-task-queue", workflowExecutionDescription.TaskQueue);
        Assert.True(workflowExecutionDescription.Memo.ContainsKey("env"));
    }
    // # --8<-- [end:UseTemporalContainer]
}
