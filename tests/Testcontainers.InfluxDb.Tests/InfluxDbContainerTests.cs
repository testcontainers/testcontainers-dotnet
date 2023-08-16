namespace Testcontainers.InfluxDb;

public sealed class InfluxDbContainerTest : IAsyncLifetime
{
    private const string Token = "test_admin_token";
    private const string Organization = "test_organization";
    private const string Bucket = "test_bucket";

    private readonly InfluxDbContainer _influxDbContainer =
        new InfluxDbBuilder()
            .WithAdminToken(Token)
            .WithOrganization(Organization)
            .WithBucket(Bucket)
            .Build();

    public Task InitializeAsync()
    {
        return _influxDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _influxDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingReturnsTrue()
    {
        // Given
        using var client = new InfluxDBClient(_influxDbContainer.GetAddress(), Token);

        // When
        var result = await client.PingAsync()
            .ConfigureAwait(false);

        // Then
        Assert.True(result);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PointQueryReturnsWrittenPoint()
    {
        // Given
        const string expectedLocationTag = "location";
        const string expectedLocationTagValue = "west";
        const double fieldValue = 55D;
        const string query = $"from(bucket:\"{Bucket}\") |> range(start: 0)";

        using var client = new InfluxDBClient(_influxDbContainer.GetAddress(), Token);
        var writeApi = client.GetWriteApiAsync();
        var queryApi = client.GetQueryApi();

        var point = PointData
            .Measurement("temperature")
            .Tag(expectedLocationTag, expectedLocationTagValue)
            .Field("value", fieldValue)
            .Timestamp(DateTime.UtcNow.AddSeconds(-10), WritePrecision.Ns);

        // When
        await writeApi.WritePointAsync(point, Bucket, Organization)
            .ConfigureAwait(false);
        var fluxTables = await queryApi.QueryAsync(query, Organization)
            .ConfigureAwait(false);

        // Then
        var recordValues = fluxTables.SingleOrDefault()?.Records?.SingleOrDefault()?.Values;
        Assert.NotNull(recordValues);
        Assert.Equal(expectedLocationTagValue, recordValues[expectedLocationTag]);
        Assert.Equal(fieldValue, recordValues["_value"]);
    }
}