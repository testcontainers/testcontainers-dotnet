namespace Testcontainers.Spanner;

public sealed class SpannerContainerTest : IAsyncLifetime
{
    // # --8<-- [start:CreateSpannerContainer]
    private readonly SpannerContainer _spannerContainer = new SpannerBuilder(TestSession.GetImageFromDockerfile()).Build();
    // # --8<-- [end:CreateSpannerContainer]

    // # --8<-- [start:UseSpannerContainer]
    public async ValueTask InitializeAsync()
    {
        await _spannerContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _spannerContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetDatabaseDdlReturnsCreateTable()
    {
        // Given
        const string projectId = "test-project";

        const string instanceId = "test-instance";

        const string databaseId = "test-database";

        const string createTable = "CREATE TABLE Users (\n  Id INT64 NOT NULL,\n  Name STRING(MAX),\n) PRIMARY KEY(Id)";

        var instanceName = new InstanceName(projectId, instanceId);

        var databaseName = new DatabaseName(projectId, instanceId, databaseId);

        var instance = new Instance();
        instance.Config = new InstanceConfigName(projectId, "emulator-config").ToString();
        instance.DisplayName = instanceId;
        instance.NodeCount = 1;

        var createDatabaseRequest = new CreateDatabaseRequest();
        createDatabaseRequest.ParentAsInstanceName = instanceName;
        createDatabaseRequest.CreateStatement = "CREATE DATABASE `" + databaseId + "`";
        createDatabaseRequest.ExtraStatements.Add(createTable);

        var instanceAdminClientBuilder = new InstanceAdminClientBuilder();
        instanceAdminClientBuilder.Endpoint = _spannerContainer.GetEmulatorGrpcEndpoint();
        instanceAdminClientBuilder.ChannelCredentials = ChannelCredentials.Insecure;

        var databaseAdminClientBuilder = new DatabaseAdminClientBuilder();
        databaseAdminClientBuilder.Endpoint = _spannerContainer.GetEmulatorGrpcEndpoint();
        databaseAdminClientBuilder.ChannelCredentials = ChannelCredentials.Insecure;

        var instanceAdminClient = await instanceAdminClientBuilder.BuildAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var databaseAdminClient = await databaseAdminClientBuilder.BuildAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var createInstanceOperation = await instanceAdminClient.CreateInstanceAsync(new ProjectName(projectId), instanceId, instance, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await createInstanceOperation.PollUntilCompletedAsync()
            .ConfigureAwait(true);

        var createDatabaseOperation = await databaseAdminClient.CreateDatabaseAsync(createDatabaseRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        _ = await createDatabaseOperation.PollUntilCompletedAsync()
            .ConfigureAwait(true);

        // When
        var databaseDdl = await databaseAdminClient.GetDatabaseDdlAsync(databaseName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(createTable, Assert.Single(databaseDdl.Statements));
        Assert.Equal(_spannerContainer.GetEmulatorGrpcEndpoint(), _spannerContainer.GetConnectionString());
    }
    // # --8<-- [end:UseSpannerContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetInstanceConfigsReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_spannerContainer.GetEmulatorHttpEndpoint());

        // When
        using var httpResponse = await httpClient.GetAsync("v1/projects/test-project/instanceConfigs", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var instanceConfigs = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Contains("projects/test-project/instanceConfigs/emulator-config", instanceConfigs);
    }
}