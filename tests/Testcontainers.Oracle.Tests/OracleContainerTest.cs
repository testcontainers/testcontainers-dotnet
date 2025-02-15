namespace Testcontainers.Oracle;

public abstract class OracleContainerTest(OracleContainerTest.OracleFixture oracleFixture)
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = oracleFixture.CreateConnection();

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1 FROM DUAL;";

        // When
        var execResult = await oracleFixture.Container.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    public abstract class OracleFixture(IMessageSink messageSink, string edition, int? version, string database = null) : DbContainerFixture<OracleBuilder, OracleContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory => OracleClientFactory.Instance;

        protected override OracleBuilder Configure(OracleBuilder builder)
        {
            if (edition == null && version == null)
            {
                return builder;
            }

            var image = $"gvenzl/oracle-{edition}:{version}-slim-faststart";
            return database == null ? builder.WithImage(image) : builder.WithImage(image).WithDatabase(database);
        }
    }

#if ORACLE_DEFAULT
    [UsedImplicitly] public sealed class OracleDefault(OracleDefaultFixture fixture) : OracleContainerTest(fixture), IClassFixture<OracleDefaultFixture>;
    [UsedImplicitly] public sealed class OracleDefaultFixture(IMessageSink messageSink) : OracleFixture(messageSink, null, null);
#endif

#if ORACLE_11
    [UsedImplicitly] public sealed class Oracle11(Oracle11Fixture fixture) : OracleContainerTest(fixture), IClassFixture<Oracle11Fixture>;
    [UsedImplicitly] public sealed class Oracle11Fixture(IMessageSink messageSink) : OracleFixture(messageSink, "xe", 11);
#endif

#if ORACLE_18
    [UsedImplicitly] public sealed class Oracle18(Oracle18Fixture fixture) : OracleContainerTest(fixture), IClassFixture<Oracle18Fixture>;
    [UsedImplicitly] public sealed class Oracle18Default(Oracle18FixtureDefault fixture) : OracleContainerTest(fixture), IClassFixture<Oracle18FixtureDefault>;
    [UsedImplicitly] public sealed class Oracle18Scott(Oracle18FixtureScott fixture) : OracleContainerTest(fixture), IClassFixture<Oracle18FixtureScott>;
    [UsedImplicitly] public sealed class Oracle18Fixture(IMessageSink messageSink) : OracleFixture(messageSink, "xe", 18);
    [UsedImplicitly] public sealed class Oracle18FixtureDefault(IMessageSink messageSink) : OracleFixture(messageSink, "xe", 18, "XEPDB1");
    [UsedImplicitly] public sealed class Oracle18FixtureScott(IMessageSink messageSink) : OracleFixture(messageSink, "xe", 18, "SCOTT");
#endif

#if ORACLE_21
    [UsedImplicitly] public sealed class Oracle21(Oracle21Fixture fixture) : OracleContainerTest(fixture), IClassFixture<Oracle21Fixture>;
    [UsedImplicitly] public sealed class Oracle21Default(Oracle21FixtureDefault fixture) : OracleContainerTest(fixture), IClassFixture<Oracle21FixtureDefault>;
    [UsedImplicitly] public sealed class Oracle21Scott(Oracle21FixtureScott fixture) : OracleContainerTest(fixture), IClassFixture<Oracle21FixtureScott>;
    [UsedImplicitly] public sealed class Oracle21Fixture(IMessageSink messageSink) : OracleFixture(messageSink, "xe", 21);
    [UsedImplicitly] public sealed class Oracle21FixtureDefault(IMessageSink messageSink) : OracleFixture(messageSink, "xe", 21, "XEPDB1");
    [UsedImplicitly] public sealed class Oracle21FixtureScott(IMessageSink messageSink) : OracleFixture(messageSink, "xe", 21, "SCOTT");
#endif

#if ORACLE_23
    [UsedImplicitly] public sealed class Oracle23(Oracle23Fixture fixture) : OracleContainerTest(fixture), IClassFixture<Oracle23Fixture>;
    [UsedImplicitly] public sealed class Oracle23Default(Oracle23FixtureDefault fixture) : OracleContainerTest(fixture), IClassFixture<Oracle23FixtureDefault>;
    [UsedImplicitly] public sealed class Oracle23Scott(Oracle23FixtureScott fixture) : OracleContainerTest(fixture), IClassFixture<Oracle23FixtureScott>;
    [UsedImplicitly] public sealed class Oracle23Fixture(IMessageSink messageSink) : OracleFixture(messageSink, "free", 23);
    [UsedImplicitly] public sealed class Oracle23FixtureDefault(IMessageSink messageSink) : OracleFixture(messageSink, "free", 23, "FREEPDB1");
    [UsedImplicitly] public sealed class Oracle23FixtureScott(IMessageSink messageSink) : OracleFixture(messageSink, "free", 23, "SCOTT");
#endif
}