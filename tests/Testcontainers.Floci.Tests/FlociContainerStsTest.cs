using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerStsTest(FlociContainerFixture fixture)
{
    private AmazonSecurityTokenServiceClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonSecurityTokenServiceConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Sts_GetCallerIdentity_ReturnsAccountAndArn()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();

        var response = await client.GetCallerIdentityAsync(new GetCallerIdentityRequest(), ct);

        Assert.False(string.IsNullOrWhiteSpace(response.Account));
        Assert.False(string.IsNullOrWhiteSpace(response.Arn));
    }
}
