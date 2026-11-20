namespace Testcontainers.Spanner;

public sealed class SpannerContainerTest : IAsyncLifetime
{
    private const string ProjectId = "test-project";

    private const string InstanceId = "test-instance";

    private const string DatabaseId = "test-database";

    private readonly SpannerContainer _spannerContainer = new SpannerBuilder(TestSession.GetImageFromDockerfile()).Build();

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
        const string name = "Testcontainers";

        var instanceName = new InstanceName(ProjectId, InstanceId);

        var databaseName = new DatabaseName(ProjectId, InstanceId, DatabaseId);

        var instance = new Instance();
        instance.Config = new InstanceConfigName(ProjectId, "emulator-config").ToString();
        instance.DisplayName = InstanceId;
        instance.NodeCount = 1;

        var createDatabaseRequest = new CreateDatabaseRequest();
        createDatabaseRequest.ParentAsInstanceName = instanceName;
        createDatabaseRequest.CreateStatement = "CREATE DATABASE `" + DatabaseId + "`";
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

        var createInstanceOperation = await instanceAdminClient.CreateInstanceAsync(new ProjectName(ProjectId), InstanceId, instance)
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
}