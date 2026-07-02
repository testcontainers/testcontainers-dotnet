# QuestDB

[QuestDB](https://questdb.com/) is a high-performance open-source time-series database designed for fast ingestion and low-latency SQL queries. It supports multiple ingestion protocols including PostgreSQL wire protocol for queries and InfluxDB Line Protocol (ILP) for high-speed time-series data ingestion.

Add the following dependency to your project file:

```shell title="NuGet"
dotnet add package Testcontainers.QuestDb
```

You can start a QuestDB container instance from any .NET application. This example uses xUnit.net's `IAsyncLifetime` interface to manage the lifecycle of the container. The container is started in the `InitializeAsync` method before the test method runs, ensuring that the environment is ready for testing. After the test completes, the container is removed in the `DisposeAsync` method.

=== "Usage Example"
    ```csharp
    --8<-- "tests/Testcontainers.QuestDb.Tests/QuestDbContainerTest.cs:UseQuestDbContainer"
    ```

The test example uses the following NuGet dependencies:

=== "Package References"
    ```xml
    --8<-- "tests/Testcontainers.QuestDb.Tests/Testcontainers.QuestDb.Tests.csproj:PackageReferences"
    ```

To execute the tests, use the command `dotnet test` from a terminal.

--8<-- "docs/modules/_call_out_test_projects.txt"

## Connection Methods

### PostgreSQL Wire Protocol (SQL Queries)

QuestDB supports the PostgreSQL wire protocol for querying data:

```csharp
var connectionString = questDbContainer.GetConnectionString();
// Returns: "Host=localhost;Port=xxxxx;Database=qdb;Username=admin;Password=quest"

using var connection = new NpgsqlConnection(connectionString);
connection.Open();

using var command = new NpgsqlCommand("SELECT * FROM sensors ORDER BY ts DESC LIMIT 10;", connection);
using var reader = command.ExecuteReader();
```

### InfluxDB Line Protocol (High-Speed Ingestion)

For high-performance time-series data ingestion, use ILP over TCP:

```csharp
var ilpHost = questDbContainer.GetInfluxLineProtocolHost();
var ilpPort = questDbContainer.GetInfluxLineProtocolPort();

using var tcpClient = new TcpClient();
await tcpClient.ConnectAsync(ilpHost, ilpPort);

using var stream = tcpClient.GetStream();
using var writer = new StreamWriter(stream) { AutoFlush = true };

// Send ILP format: measurement,tags fields timestamp
await writer.WriteLineAsync("sensors,device_id=001,location=warehouse temperature=22.5,humidity=65.2");
```

**ILP Format:**
```
measurement,tag1=value1,tag2=value2 field1=value1,field2=value2 timestamp
```

### REST API

For direct REST API access:

```csharp
var restApiAddress = questDbContainer.GetRestApiAddress();

using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(restApiAddress);

// Execute SQL via REST
var response = await httpClient.GetAsync("/exec?query=SELECT * FROM sensors");
```

### Web Console

Access the interactive Web Console:

```csharp
var webConsoleUrl = questDbContainer.GetWebConsoleUrl();
// Open in browser: http://localhost:xxxxx
```

## Configuration

### Custom Credentials

```csharp
var questDbContainer = new QuestDbBuilder("questdb/questdb:9.2.3")
    .WithUsername("myuser")
    .WithPassword("mypassword")
    .Build();
```

## Example: Combined SQL + ILP

```csharp
// 1. Create table via SQL
await using var connection = new NpgsqlConnection(questDbContainer.GetConnectionString());
await connection.OpenAsync();

await using var createCommand = new NpgsqlCommand(
    "CREATE TABLE IF NOT EXISTS sensors (ts TIMESTAMP, device_id SYMBOL, temperature DOUBLE, humidity DOUBLE) timestamp(ts) PARTITION BY DAY;",
    connection);
await createCommand.ExecuteNonQueryAsync();

// 2. Ingest data via ILP (high-speed)
using var tcpClient = new TcpClient();
await tcpClient.ConnectAsync(
    questDbContainer.GetInfluxLineProtocolHost(),
    questDbContainer.GetInfluxLineProtocolPort());

using var stream = tcpClient.GetStream();
using var writer = new StreamWriter(stream) { AutoFlush = true };

for (int i = 0; i < 10000; i++)
{
    await writer.WriteLineAsync($"sensors,device_id=dev{i % 10} temperature={20 + i % 30},humidity={50 + i % 40}");
}

// 3. Query results via SQL
await using var queryCommand = new NpgsqlCommand(
    @"SELECT device_id,
             AVG(temperature) as avg_temp,
             AVG(humidity) as avg_humidity
      FROM sensors
      WHERE ts > dateadd('h', -1, now())
      GROUP BY device_id;",
    connection);

await using var reader = await queryCommand.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    var deviceId = reader.GetString(0);
    var avgTemp = reader.GetDouble(1);
    var avgHumidity = reader.GetDouble(2);
    Console.WriteLine($"{deviceId}: {avgTemp}°C, {avgHumidity}%");
}
```

## Time-Series Features

QuestDB extends SQL with powerful time-series operators:

### SAMPLE BY (Downsampling)
```sql
SELECT ts, AVG(temperature)
FROM sensors
WHERE ts > dateadd('d', -7, now())
SAMPLE BY 1h;
```

### LATEST ON (Deduplication)
```sql
SELECT *
FROM sensors
LATEST ON ts PARTITION BY device_id;
```

### ASOF JOIN (Time-series Join)
```sql
SELECT *
FROM trades
ASOF JOIN quotes
ON symbol;
```

## Protocol Selection Guide

| Protocol | Use Case | Performance | Complexity |
|----------|----------|-------------|------------|
| **ILP (TCP)** | High-speed ingestion | ⚡ Fastest | Simple |
| **PostgreSQL** | SQL queries, transactions | Fast | Standard SQL |
| **REST API** | Ad-hoc queries, web apps | Moderate | JSON/HTTP |

**Recommendation:** Use ILP for ingestion, PostgreSQL for queries.

## References
- [QuestDB Documentation](https://questdb.com/docs/)
- [QuestDB ILP Reference](https://questdb.com/docs/reference/api/ilp/overview/)
- [QuestDB Docker Hub](https://hub.docker.com/r/questdb/questdb)
- [Npgsql - .NET PostgreSQL Client](https://www.npgsql.org/)
