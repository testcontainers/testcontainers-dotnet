using System;
using System.Threading.Tasks;
using Amazon.ECR;
using Amazon.ECR.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerEcrTest(FlociContainerFixture fixture)
{
    private AmazonECRClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonECRConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact(Skip = "Floci ECR requires Docker-in-Docker (backing registry container); not supported in this environment")]
    public async Task Ecr_CreateAndDescribeRepository_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"repo-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var created = await client.CreateRepositoryAsync(new CreateRepositoryRequest { RepositoryName = name }, ct);
        var response = await client.DescribeRepositoriesAsync(new DescribeRepositoriesRequest { RepositoryNames = [name] }, ct);

        Assert.False(string.IsNullOrWhiteSpace(created.Repository.RepositoryArn));
        Assert.Single(response.Repositories);
        Assert.Equal(name, response.Repositories[0].RepositoryName);
    }

    [Fact(Skip = "Floci ECR requires Docker-in-Docker (backing registry container); not supported in this environment")]
    public async Task Ecr_DeleteRepository_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"repo-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateRepositoryAsync(new CreateRepositoryRequest { RepositoryName = name }, ct);
        await client.DeleteRepositoryAsync(new DeleteRepositoryRequest { RepositoryName = name, Force = true }, ct);
        var response = await client.DescribeRepositoriesAsync(new DescribeRepositoriesRequest(), ct);

        Assert.DoesNotContain(response.Repositories, r => r.RepositoryName == name);
    }
}
