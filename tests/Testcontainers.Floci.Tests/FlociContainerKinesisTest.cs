using System;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerKinesisTest(FlociContainerFixture fixture)
{
    private AmazonKinesisClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonKinesisConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Kinesis_CreateAndListStream_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"stream-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateStreamAsync(new CreateStreamRequest { StreamName = name, ShardCount = 1 }, ct);
        var response = await client.ListStreamsAsync(new ListStreamsRequest(), ct);

        Assert.Contains(name, response.StreamNames);
    }

    [Fact]
    public async Task Kinesis_DescribeStream_ReturnsArn()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"stream-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateStreamAsync(new CreateStreamRequest { StreamName = name, ShardCount = 1 }, ct);
        var response = await client.DescribeStreamAsync(new DescribeStreamRequest { StreamName = name }, ct);

        Assert.Contains(name, response.StreamDescription.StreamARN);
    }
}
