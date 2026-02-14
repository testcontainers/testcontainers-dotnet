namespace Testcontainers.Temporalio;

public sealed class TemporalContainerWorkflowTest : IAsyncLifetime
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
    public async Task StartWorkflowThenListAndDescribe()
    {
        // Given
        var client = await TemporalClient.ConnectAsync(new(_temporalContainer.GetGrpcAddress())
        {
            Namespace = "default",
        }).ConfigureAwait(true);

        var workflowId = Guid.NewGuid().ToString("D");

        // When — start a workflow (equivalent to: temporal workflow start)
        var handle = await client.StartWorkflowAsync("MyWorkflow", Array.Empty<object>(), new(id: workflowId, taskQueue: "my-task-queue"))
            .ConfigureAwait(true);

        // Then — list workflows (equivalent to: temporal workflow list)
        var workflows = new List<WorkflowExecution>();

        await foreach (var execution in client.ListWorkflowsAsync($"WorkflowId = '{workflowId}'").ConfigureAwait(true))
        {
            workflows.Add(execution);
        }

        Assert.Single(workflows);
        Assert.Equal(workflowId, workflows[0].Id);
        Assert.Equal("MyWorkflow", workflows[0].WorkflowType);

        // Then — describe workflow (equivalent to: temporal workflow describe)
        var description = await handle.DescribeAsync().ConfigureAwait(true);

        Assert.Equal(workflowId, description.Id);
        Assert.Equal("MyWorkflow", description.WorkflowType);
        Assert.Equal("my-task-queue", description.TaskQueue);
        Assert.NotNull(description.RunId);
    }
}
