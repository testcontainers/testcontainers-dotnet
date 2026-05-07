using System;
using System.Threading.Tasks;
using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerCloudFormationTest(FlociContainerFixture fixture)
{
    private AmazonCloudFormationClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonCloudFormationConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    private const string Template = """
        {
          "AWSTemplateFormatVersion": "2010-09-09",
          "Resources": {
            "TestBucket": {
              "Type": "AWS::S3::Bucket"
            }
          }
        }
        """;

    [Fact]
    public async Task CloudFormation_CreateAndListStack_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"stack-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateStackAsync(new CreateStackRequest { StackName = name, TemplateBody = Template }, ct);
        var response = await client.ListStacksAsync(new ListStacksRequest(), ct);

        Assert.Contains(response.StackSummaries, s => s.StackName == name);
    }

    [Fact]
    public async Task CloudFormation_DescribeStack_ReturnsExpectedName()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"stack-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateStackAsync(new CreateStackRequest { StackName = name, TemplateBody = Template }, ct);
        var response = await client.DescribeStacksAsync(new DescribeStacksRequest { StackName = name }, ct);

        Assert.Contains(response.Stacks, s => s.StackName == name);
    }
}
