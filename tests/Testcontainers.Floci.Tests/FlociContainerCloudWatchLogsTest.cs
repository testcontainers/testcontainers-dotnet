using System;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerCloudWatchLogsTest(FlociContainerFixture fixture)
{
    private AmazonCloudWatchLogsClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonCloudWatchLogsConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task CloudWatchLogs_CreateLogGroupAndDescribe_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var groupName = $"/test/group-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateLogGroupAsync(new CreateLogGroupRequest { LogGroupName = groupName }, ct);
        var response = await client.DescribeLogGroupsAsync(new DescribeLogGroupsRequest { LogGroupNamePrefix = groupName }, ct);

        Assert.Contains(response.LogGroups, g => g.LogGroupName == groupName);
    }

    [Fact]
    public async Task CloudWatchLogs_PutAndGetLogEvents_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var groupName = $"/test/group-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        const string streamName = "test-stream";
        const string message = "Hello from Floci CloudWatch!";

        await client.CreateLogGroupAsync(new CreateLogGroupRequest { LogGroupName = groupName }, ct);
        await client.CreateLogStreamAsync(new CreateLogStreamRequest { LogGroupName = groupName, LogStreamName = streamName }, ct);
        await client.PutLogEventsAsync(new PutLogEventsRequest
        {
            LogGroupName = groupName,
            LogStreamName = streamName,
            LogEvents = [new InputLogEvent { Message = message, Timestamp = DateTime.UtcNow }],
        }, ct);
        var response = await client.GetLogEventsAsync(new GetLogEventsRequest { LogGroupName = groupName, LogStreamName = streamName }, ct);

        Assert.Contains(response.Events, e => e.Message == message);
    }
}
