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
    public async Task SelectReturnsInsertedRow()
    {
        // Given
        const string projectId = "test-project";

        const string instanceId = "test-instance";

        const string databaseId = "test-database";

        const string name = "Testcontainers";

        var instanceName = new InstanceName(projectId, instanceId);

        var databaseName = new DatabaseName(projectId, instanceId, databaseId);

        var instance = new Instance();
        instance.Config = new InstanceConfigName(projectId, "emulator-config").ToString();
        instance.DisplayName = instanceId;
        instance.NodeCount = 1;

        var createDatabaseRequest = new CreateDatabaseRequest();
        createDatabaseRequest.ParentAsInstanceName = instanceName;
        createDatabaseRequest.CreateStatement = "CREATE DATABASE `" + databaseId + "`";
        createDatabaseRequest.ExtraStatements.Add("CREATE TABLE Users (Id INT64 NOT NULL, Name STRING(MAX)) PRIMARY KEY (Id)");

        var instanceAdminClientBuilder = new InstanceAdminClientBuilder();
        instanceAdminClientBuilder.Endpoint = _spannerContainer.GetEmulatorGrpcEndpoint();
        instanceAdminClientBuilder.ChannelCredentials = ChannelCredentials.Insecure;

        var databaseAdminClientBuilder = new DatabaseAdminClientBuilder();
        databaseAdminClientBuilder.Endpoint = _spannerContainer.GetEmulatorGrpcEndpoint();
        databaseAdminClientBuilder.ChannelCredentials = ChannelCredentials.Insecure;

        var connectionStringBuilder = new SpannerConnectionStringBuilder("Data Source=" + databaseName, ChannelCredentials.Insecure);
        connectionStringBuilder.Host = _spannerContainer.Hostname;
        connectionStringBuilder.Port = _spannerContainer.GetMappedPublicPort(SpannerBuilder.SpannerGrpcPort);

        var instanceAdminClient = await instanceAdminClientBuilder.BuildAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var databaseAdminClient = await databaseAdminClientBuilder.BuildAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var createInstanceOperation = await instanceAdminClient.CreateInstanceAsync(new ProjectName(projectId), instanceId, instance)
            .ConfigureAwait(true);

        _ = await createInstanceOperation.PollUntilCompletedAsync()
            .ConfigureAwait(true);

        var createDatabaseOperation = await databaseAdminClient.CreateDatabaseAsync(createDatabaseRequest)
            .ConfigureAwait(true);

        _ = await createDatabaseOperation.PollUntilCompletedAsync()
            .ConfigureAwait(true);

        await using var connection = new SpannerConnection(connectionStringBuilder);

        var parameters = new SpannerParameterCollection();
        parameters.Add("Id", SpannerDbType.Int64, 1);
        parameters.Add("Name", SpannerDbType.String, name);

        using var insertCommand = connection.CreateInsertCommand("Users", parameters);

        _ = await insertCommand.ExecuteNonQueryAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        using var selectCommand = connection.CreateSelectCommand("SELECT Name FROM Users WHERE Id = 1");

        var actualName = await selectCommand.ExecuteScalarAsync<string>(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(name, actualName);
        Assert.Equal(_spannerContainer.GetEmulatorGrpcEndpoint(), _spannerContainer.GetConnectionString());
    }
    // # --8<-- [end:UseSpannerContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetInstanceConfigsReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Uri.UriSchemeHttp + "://" + _spannerContainer.GetEmulatorHttpEndpoint());

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