namespace Testcontainers.Temporalio;

public sealed class TemporalContainerCustomNamespaceTest : IAsyncLifetime
{
    private readonly TemporalContainer _temporalContainer = new TemporalBuilder(TestSession.GetImageFromDockerfile())
        .WithNamespace("custom-namespace")
        .Build();

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
    public async Task CustomNamespaceIsPreRegistered()
    {
        // Given
        var client = await TemporalClient.ConnectAsync(new(_temporalContainer.GetGrpcAddress())
        {
            Namespace = "custom-namespace",
        }).ConfigureAwait(true);

        // When
        var response = await client.Connection.WorkflowService.ListNamespacesAsync(new()).ConfigureAwait(true);

        // Then
        Assert.Contains(response.Namespaces, ns => ns.NamespaceInfo.Name == "custom-namespace");
        Assert.Contains(response.Namespaces, ns => ns.NamespaceInfo.Name == "default");
    }
}

public sealed class TemporalContainerSearchAttributeTest : IAsyncLifetime
{
    private readonly TemporalContainer _temporalContainer = new TemporalBuilder(TestSession.GetImageFromDockerfile())
        .WithSearchAttribute("CustomerId", "Keyword")
        .WithSearchAttribute("OrderDate", "Datetime")
        .Build();

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
    public async Task CustomSearchAttributesAreRegistered()
    {
        // Given
        var client = await TemporalClient.ConnectAsync(new(_temporalContainer.GetGrpcAddress())
        {
            Namespace = "default",
        }).ConfigureAwait(true);

        // When
        var response = await client.Connection.OperatorService.ListSearchAttributesAsync(
            new() { Namespace = "default" }).ConfigureAwait(true);

        // Then
        Assert.True(response.CustomAttributes.ContainsKey("CustomerId"));
        Assert.True(response.CustomAttributes.ContainsKey("OrderDate"));
    }
}

public sealed class TemporalContainerDynamicConfigTest : IAsyncLifetime
{
    private readonly TemporalContainer _temporalContainer = new TemporalBuilder(TestSession.GetImageFromDockerfile())
        .WithDynamicConfigValue("limit.maxIDLength", "10")
        .Build();

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
    public async Task DynamicConfigValueLimitsWorkflowIdLength()
    {
        // Given
        var client = await TemporalClient.ConnectAsync(new(_temporalContainer.GetGrpcAddress())
        {
            Namespace = "default",
        }).ConfigureAwait(true);

        var longWorkflowId = new string('x', 50);

        // When / Then — starting a workflow with an ID exceeding maxIDLength should fail
        var exception = await Assert.ThrowsAsync<global::Temporalio.Exceptions.RpcException>(() =>
            client.StartWorkflowAsync("TestWorkflow", Array.Empty<object>(), new(id: longWorkflowId, taskQueue: "test-queue")))
            .ConfigureAwait(true);

        Assert.Contains("length exceeds limit", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class TemporalContainerDbFilenameTest : IAsyncLifetime
{
    private readonly TemporalContainer _temporalContainer = new TemporalBuilder(TestSession.GetImageFromDockerfile())
        .WithDbFilename("/tmp/temporal.db")
        .Build();

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
    public async Task PersistentWorkflowSurvivesContainerRestart()
    {
        // Given
        var client = await TemporalClient.ConnectAsync(new(_temporalContainer.GetGrpcAddress())
        {
            Namespace = "default",
        }).ConfigureAwait(true);

        var workflowId = Guid.NewGuid().ToString("D");

        await client.StartWorkflowAsync("PersistentWorkflow", Array.Empty<object>(), new(id: workflowId, taskQueue: "test-queue"))
            .ConfigureAwait(true);

        // When
        await _temporalContainer.StopAsync(CancellationToken.None).ConfigureAwait(true);
        await _temporalContainer.StartAsync(CancellationToken.None).ConfigureAwait(true);

        // Then
        var clientAfterRestart = await TemporalClient.ConnectAsync(new(_temporalContainer.GetGrpcAddress())
        {
            Namespace = "default",
        }).ConfigureAwait(true);

        var handle = clientAfterRestart.GetWorkflowHandle(workflowId);
        var description = await handle.DescribeAsync().ConfigureAwait(true);

        Assert.Equal(workflowId, description.Id);
        Assert.Equal("PersistentWorkflow", description.WorkflowType);
    }
}
