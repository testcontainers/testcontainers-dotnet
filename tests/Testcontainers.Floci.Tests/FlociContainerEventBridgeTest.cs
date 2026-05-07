using System;
using System.Threading.Tasks;
using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerEventBridgeTest(FlociContainerFixture fixture)
{
    private AmazonEventBridgeClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonEventBridgeConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task EventBridge_PutAndListRule_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"rule-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.PutRuleAsync(new PutRuleRequest
        {
            Name = name,
            EventPattern = """{"source":["my.app"]}""",
            State = RuleState.ENABLED,
        }, ct);
        var response = await client.ListRulesAsync(new ListRulesRequest(), ct);

        Assert.Contains(response.Rules, r => r.Name == name);
    }

    [Fact]
    public async Task EventBridge_PutEvents_SucceedsWithNoFailures()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();

        var response = await client.PutEventsAsync(new PutEventsRequest
        {
            Entries =
            [
                new PutEventsRequestEntry
                {
                    Source = "my.app",
                    DetailType = "TestEvent",
                    Detail = """{"key":"value"}""",
                },
            ],
        }, ct);

        Assert.Equal(0, response.FailedEntryCount);
    }
}
