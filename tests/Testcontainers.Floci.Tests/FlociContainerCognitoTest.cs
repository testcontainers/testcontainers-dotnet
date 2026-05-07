using System;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerCognitoTest(FlociContainerFixture fixture)
{
    private AmazonCognitoIdentityProviderClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonCognitoIdentityProviderConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Cognito_CreateAndListUserPool_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"pool-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var created = await client.CreateUserPoolAsync(new CreateUserPoolRequest { PoolName = name }, ct);
        var response = await client.ListUserPoolsAsync(new ListUserPoolsRequest { MaxResults = 10 }, ct);

        Assert.False(string.IsNullOrWhiteSpace(created.UserPool.Id));
        Assert.Contains(response.UserPools, p => p.Name == name);
    }

    [Fact]
    public async Task Cognito_DescribeUserPool_ReturnsExpectedName()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"pool-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var created = await client.CreateUserPoolAsync(new CreateUserPoolRequest { PoolName = name }, ct);
        var response = await client.DescribeUserPoolAsync(new DescribeUserPoolRequest { UserPoolId = created.UserPool.Id }, ct);

        Assert.Equal(name, response.UserPool.Name);
    }
}
