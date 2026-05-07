using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerSesTest(FlociContainerFixture fixture)
{
    private AmazonSimpleEmailServiceClient CreateClient() =>
        new(new BasicAWSCredentials(fixture.Container.GetAccessKey(), fixture.Container.GetSecretKey()),
            new AmazonSimpleEmailServiceConfig
            {
                ServiceURL = fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Ses_VerifyEmailIdentity_AppearsInList()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var email = $"test-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}@example.com";

        await client.VerifyEmailIdentityAsync(new VerifyEmailIdentityRequest { EmailAddress = email }, ct);
        var response = await client.ListIdentitiesAsync(new ListIdentitiesRequest { IdentityType = IdentityType.EmailAddress }, ct);

        Assert.Contains(email, response.Identities);
    }

    [Fact]
    public async Task Ses_ListIdentities_ReturnsNonNullList()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();
        var email = $"test-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}@example.com";

        await client.VerifyEmailIdentityAsync(new VerifyEmailIdentityRequest { EmailAddress = email }, ct);
        var response = await client.ListIdentitiesAsync(new ListIdentitiesRequest(), ct);

        Assert.NotNull(response.Identities);
    }
}
