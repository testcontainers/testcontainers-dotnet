using System;
using System.Threading.Tasks;
using Amazon.KinesisFirehose;
using Amazon.KinesisFirehose.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerFirehoseTest(FlociContainerFixture fixture)
{
    private AmazonKinesisFirehoseClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonKinesisFirehoseConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Firehose_CreateAndListDeliveryStream_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"stream-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateDeliveryStreamAsync(new CreateDeliveryStreamRequest
        {
            DeliveryStreamName = name,
            DeliveryStreamType = DeliveryStreamType.DirectPut,
        }, ct);
        var response = await client.ListDeliveryStreamsAsync(new ListDeliveryStreamsRequest(), ct);

        Assert.Contains(name, response.DeliveryStreamNames);
    }

    [Fact]
    public async Task Firehose_DescribeDeliveryStream_ReturnsArn()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"stream-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateDeliveryStreamAsync(new CreateDeliveryStreamRequest
        {
            DeliveryStreamName = name,
            DeliveryStreamType = DeliveryStreamType.DirectPut,
        }, ct);
        var response = await client.DescribeDeliveryStreamAsync(new DescribeDeliveryStreamRequest { DeliveryStreamName = name }, ct);

        Assert.Contains(name, response.DeliveryStreamDescription.DeliveryStreamARN);
    }
}
