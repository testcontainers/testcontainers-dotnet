using System.Threading.Tasks;
using Amazon.ResourceGroupsTaggingAPI;
using Amazon.ResourceGroupsTaggingAPI.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerResourceGroupsTaggingTest(FlociContainerFixture fixture)
{
    private AmazonResourceGroupsTaggingAPIClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonResourceGroupsTaggingAPIConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task ResourceGroupsTagging_GetResources_ReturnsNonNullList()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();

        var response = await client.GetResourcesAsync(new GetResourcesRequest(), ct);

        Assert.NotNull(response.ResourceTagMappingList);
    }

    [Fact]
    public async Task ResourceGroupsTagging_GetResourcesWithTagFilter_ReturnsNonNullList()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();

        var response = await client.GetResourcesAsync(new GetResourcesRequest
        {
            TagFilters = [new TagFilter { Key = "Environment", Values = ["test"] }],
        }, ct);

        Assert.NotNull(response.ResourceTagMappingList);
    }
}
