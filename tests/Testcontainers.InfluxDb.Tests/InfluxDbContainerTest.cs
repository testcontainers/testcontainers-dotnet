namespace Testcontainers.InfluxDb;

public sealed class InfluxDbContainerTest : IAsyncLifetime
{
    private const string AdminToken = "YOUR_API_TOKEN";

    private readonly InfluxDbContainer _influxDbContainer = new InfluxDbBuilder().WithAdminToken(AdminToken).Build();

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
        using var client = new InfluxDBClient(_influxDbContainer.GetAddress(), AdminToken);

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
        const string query = "from(bucket:\"" + InfluxDbBuilder.DefaultBucket + "\") |> range(start: 0)";

        const string regionTagName = "region";

        const string regionTagValue = "eu-central-1";

        const double temperature = 55d;

        using var client = new InfluxDBClient(_influxDbContainer.GetAddress(), AdminToken);
        var queryApi = client.GetQueryApi();
        var writeApi = client.GetWriteApiAsync();

        var point = PointData
            .Measurement("temperature")
            .Field("value", temperature)
            .Tag(regionTagName, regionTagValue)
            .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

        // When
        await writeApi.WritePointAsync(point, InfluxDbBuilder.DefaultBucket, InfluxDbBuilder.DefaultOrganization)
            .ConfigureAwait(false);

        var fluxTables = await queryApi.QueryAsync(query, InfluxDbBuilder.DefaultOrganization)
            .ConfigureAwait(false);

        var recordValues = fluxTables.Single().Records.Single().Values;

        // Then
        Assert.Equal(regionTagValue, recordValues[regionTagName]);
        Assert.Equal(temperature, recordValues["_value"]);
    }
}