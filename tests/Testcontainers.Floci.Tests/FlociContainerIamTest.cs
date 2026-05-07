using System;
using System.Threading.Tasks;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerIamTest(FlociContainerFixture fixture)
{
    private AmazonIdentityManagementServiceClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonIdentityManagementServiceConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Iam_CreateAndListUser_Succeeds()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"user-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await client.CreateUserAsync(new CreateUserRequest { UserName = name }, ct);
        var response = await client.ListUsersAsync(new ListUsersRequest(), ct);

        Assert.Contains(response.Users, u => u.UserName == name);
    }

    [Fact]
    public async Task Iam_CreateAndGetRole_ReturnsExpectedArn()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var name = $"role-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        const string policy = """{"Version":"2012-10-17","Statement":[{"Effect":"Allow","Principal":{"Service":"lambda.amazonaws.com"},"Action":"sts:AssumeRole"}]}""";

        await client.CreateRoleAsync(new CreateRoleRequest { RoleName = name, AssumeRolePolicyDocument = policy }, ct);
        var response = await client.GetRoleAsync(new GetRoleRequest { RoleName = name }, ct);

        Assert.Contains(name, response.Role.Arn);
    }
}
