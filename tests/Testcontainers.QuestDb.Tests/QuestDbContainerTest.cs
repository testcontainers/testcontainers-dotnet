namespace Testcontainers.QuestDb;

public abstract class QuestDbContainerTest(QuestDbContainerTest.QuestDbDefaultFixture fixture)
{
    // # --8<-- [start:UseQuestDbContainer]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = fixture.CreateConnection();

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(fixture.Container.GetConnectionString(), fixture.Container.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task IlpIngestReturnsRecord()
    {
        // Given
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        var ilpAddress = new Uri(fixture.Container.GetIlpAddress());

        using var sender = Sender.New($"tcp::addr={ilpAddress.Host}:{ilpAddress.Port};");

        await using var createTableCommand = fixture.CreateCommand("CREATE TABLE sensors (id SYMBOL, temperature DOUBLE, ts TIMESTAMP) TIMESTAMP(ts) PARTITION BY DAY WAL;");

        await using var selectTemperatureCommand = fixture.CreateCommand("SELECT temperature FROM sensors;");

        _ = await createTableCommand.ExecuteNonQueryAsync(cts.Token)
            .ConfigureAwait(true);

        // When
        await sender.Table("sensors").Symbol("id", "1").Column("temperature", 21.5).AtNowAsync(cts.Token)
            .ConfigureAwait(true);

        await sender.SendAsync(cts.Token)
            .ConfigureAwait(true);

        // Then
        object temperature;

        do
        {
            temperature = await selectTemperatureCommand.ExecuteScalarAsync(cts.Token)
                .ConfigureAwait(true);
        }
        while (temperature == null);

        Assert.Equal(21.5, temperature);
    }
    // # --8<-- [end:UseQuestDbContainer]

    public class QuestDbDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<QuestDbBuilder, QuestDbContainer>(messageSink)
    {
        protected override QuestDbBuilder Configure()
            => new QuestDbBuilder(TestSession.GetImageFromDockerfile());

        public override DbProviderFactory DbProviderFactory
            => NpgsqlFactory.Instance;
    }

    [UsedImplicitly]
    public class QuestDbWaitForDatabaseFixture(IMessageSink messageSink)
        : QuestDbDefaultFixture(messageSink)
    {
        protected override QuestDbBuilder Configure()
            => base.Configure().WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public sealed class QuestDbDefaultConfiguration(QuestDbDefaultFixture fixture)
        : QuestDbContainerTest(fixture), IClassFixture<QuestDbDefaultFixture>;

    [UsedImplicitly]
    public sealed class QuestDbWaitForDatabaseConfiguration(QuestDbWaitForDatabaseFixture fixture)
        : QuestDbContainerTest(fixture), IClassFixture<QuestDbWaitForDatabaseFixture>;
}