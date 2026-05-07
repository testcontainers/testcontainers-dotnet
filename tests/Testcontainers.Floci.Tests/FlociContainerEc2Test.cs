using System;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerEc2Test(FlociContainerFixture fixture)
{
    private AmazonEC2Client CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonEC2Config
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Ec2_CreateAndDescribeVpc_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();

        var created = await client.CreateVpcAsync(new CreateVpcRequest { CidrBlock = "10.0.0.0/16" }, ct);
        var vpcId = created.Vpc.VpcId;
        var response = await client.DescribeVpcsAsync(new DescribeVpcsRequest { VpcIds = [vpcId] }, ct);

        Assert.Single(response.Vpcs);
        Assert.Equal("10.0.0.0/16", response.Vpcs[0].CidrBlock);
    }

    [Fact]
    public async Task Ec2_CreateAndDescribeSecurityGroup_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var groupName = $"sg-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var vpc = await client.CreateVpcAsync(new CreateVpcRequest { CidrBlock = "10.1.0.0/16" }, ct);
        var created = await client.CreateSecurityGroupAsync(new CreateSecurityGroupRequest
        {
            VpcId = vpc.Vpc.VpcId,
            GroupName = groupName,
            Description = "Test security group",
        }, ct);
        var response = await client.DescribeSecurityGroupsAsync(new DescribeSecurityGroupsRequest { GroupIds = [created.GroupId] }, ct);

        Assert.Single(response.SecurityGroups);
        Assert.Equal(groupName, response.SecurityGroups[0].GroupName);
    }
}
