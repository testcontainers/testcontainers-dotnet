namespace Testcontainers.QuestDb;

public sealed class QuestDbContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseQuestDbContainer]
    private readonly QuestDbContainer _questDbContainer = new QuestDbBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _questDbContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _questDbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using var connection = new NpgsqlConnection(_questDbContainer.GetConnectionString());

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }
    // # --8<-- [end:UseQuestDbContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecuteQueryReturnsResults()
    {
        // Given
        await using var connection = new NpgsqlConnection(_questDbContainer.GetConnectionString());
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        await using var command = new NpgsqlCommand("CREATE TABLE test (ts TIMESTAMP, value DOUBLE) timestamp(ts);", connection);
        await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);

        // When
        await using var insertCommand = new NpgsqlCommand("INSERT INTO test VALUES (now(), 42.0);", connection);
        await insertCommand.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);

        await using var selectCommand = new NpgsqlCommand("SELECT * FROM test;", connection);
        await using var reader = await selectCommand.ExecuteReaderAsync(TestContext.Current.CancellationToken);

        // Then
        Assert.True(await reader.ReadAsync(TestContext.Current.CancellationToken));
        Assert.Equal(42.0, reader.GetDouble(1));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task RestApiReturnsOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_questDbContainer.GetRestApiAddress());

        // When
        using var response = await httpClient.GetAsync("/", TestContext.Current.CancellationToken);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task InfluxLineProtocolIngestReturnsSuccess()
    {
        // Given
        var ilpHost = _questDbContainer.GetInfluxLineProtocolHost();
        var ilpPort = _questDbContainer.GetInfluxLineProtocolPort();

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(ilpHost, ilpPort, TestContext.Current.CancellationToken);

        await using var stream = tcpClient.GetStream();
        await using var writer = new StreamWriter(stream) { AutoFlush = true };

        // When - Send ILP line
        await writer.WriteLineAsync("sensors,device_id=001 temperature=22.5");
        await Task.Delay(2000, TestContext.Current.CancellationToken); // QuestDB commits ILP data in batches (default commit lag ~1s)

        // Then - Verify via SQL
        await using var connection = new NpgsqlConnection(_questDbContainer.GetConnectionString());
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        await using var command = new NpgsqlCommand("SELECT * FROM sensors WHERE device_id = '001';", connection);
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);

        Assert.True(await reader.ReadAsync(TestContext.Current.CancellationToken));
        Assert.Equal("001", reader.GetString(reader.GetOrdinal("device_id")));
        Assert.Equal(22.5, reader.GetDouble(reader.GetOrdinal("temperature")));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task InfluxLineProtocolBulkIngest()
    {
        // Given
        var ilpHost = _questDbContainer.GetInfluxLineProtocolHost();
        var ilpPort = _questDbContainer.GetInfluxLineProtocolPort();

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(ilpHost, ilpPort, TestContext.Current.CancellationToken);

        await using var stream = tcpClient.GetStream();
        await using var writer = new StreamWriter(stream) { AutoFlush = true };

        // When - Bulk ingest
        for (int i = 0; i < 100; i++)
        {
            await writer.WriteLineAsync($"metrics,sensor=A value={i}");
        }

        await Task.Delay(2000, TestContext.Current.CancellationToken); // QuestDB commits ILP data in batches (default commit lag ~1s)

        // Then - Verify count
        await using var connection = new NpgsqlConnection(_questDbContainer.GetConnectionString());
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        await using var command = new NpgsqlCommand("SELECT COUNT(*) FROM metrics WHERE sensor = 'A';", connection);
        var count = (long)await command.ExecuteScalarAsync(TestContext.Current.CancellationToken);

        Assert.Equal(100L, count);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task QuestDbClientIngestReturnsSuccess()
    {
        // Given - Official QuestDB .NET client
        var configString = _questDbContainer.GetClientConnectionString(useHttpTransport: true);

        using var sender = Sender.New(configString);

        // When - Send data using official client
        await sender.Table("trades")
            .Symbol("symbol", "BTC-USD")
            .Symbol("side", "buy")
            .Column("price", 45000.50)
            .Column("amount", 0.5)
            .AtNowAsync(TestContext.Current.CancellationToken);

        await sender.SendAsync(TestContext.Current.CancellationToken);
        await Task.Delay(2000, TestContext.Current.CancellationToken); // QuestDB commits ILP data in batches

        // Then - Verify via SQL
        await using var connection = new NpgsqlConnection(_questDbContainer.GetConnectionString());
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        await using var command = new NpgsqlCommand("SELECT * FROM trades WHERE symbol = 'BTC-USD';", connection);
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);

        Assert.True(await reader.ReadAsync(TestContext.Current.CancellationToken));
        Assert.Equal("BTC-USD", reader.GetString(reader.GetOrdinal("symbol")));
        Assert.Equal("buy", reader.GetString(reader.GetOrdinal("side")));
        Assert.Equal(45000.50, reader.GetDouble(reader.GetOrdinal("price")));
        Assert.Equal(0.5, reader.GetDouble(reader.GetOrdinal("amount")));
    }
}
