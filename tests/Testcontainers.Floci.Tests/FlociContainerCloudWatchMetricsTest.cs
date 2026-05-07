using System;
using System.Threading.Tasks;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerCloudWatchMetricsTest(FlociContainerFixture fixture)
{
    private AmazonCloudWatchClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonCloudWatchConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task CloudWatch_PutMetricData_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();

        await client.PutMetricDataAsync(new PutMetricDataRequest
        {
            Namespace = "TestNamespace",
            MetricData =
            [
                new MetricDatum
                {
                    MetricName = "TestMetric",
                    Value = 42.0,
                    Unit = StandardUnit.Count,
                    Timestamp = DateTime.UtcNow,
                    Dimensions = [new Dimension { Name = "TestDim", Value = "TestValue" }],
                },
            ],
        }, ct);
    }

    [Fact]
    public async Task CloudWatch_ListMetrics_ReturnsPublishedMetric()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var ns = $"ListTest-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.PutMetricDataAsync(new PutMetricDataRequest
        {
            Namespace = ns,
            MetricData = [new MetricDatum { MetricName = "Metric1", Value = 1.0, Unit = StandardUnit.Count, Timestamp = DateTime.UtcNow }],
        }, ct);
        var response = await client.ListMetricsAsync(new ListMetricsRequest { Namespace = ns }, ct);

        Assert.Contains(response.Metrics, m => m.MetricName == "Metric1");
    }
}
