using System.Threading.Tasks;
using Amazon.CertificateManager;
using Amazon.CertificateManager.Model;
using Amazon.Runtime;
using Xunit;

namespace Testcontainers.Floci.Tests;

[Collection("Floci")]
public sealed class FlociContainerAcmTest
{
    private readonly FlociContainerFixture _fixture;

    public FlociContainerAcmTest(FlociContainerFixture fixture) => _fixture = fixture;

    private AmazonCertificateManagerClient CreateClient() =>
        new(new BasicAWSCredentials(_fixture.Container.GetAccessKey(), _fixture.Container.GetSecretKey()),
            new AmazonCertificateManagerConfig
            {
                ServiceURL = _fixture.Container.GetConnectionString(),
                AuthenticationRegion = FlociContainer.Region,
            });

    [Fact]
    public async Task Acm_ListCertificates_ReturnsNonNullList()
    {
        var ct = TestContext.Current.CancellationToken;
        using var client = CreateClient();

        var response = await client.ListCertificatesAsync(new ListCertificatesRequest(), ct);

        Assert.NotNull(response.CertificateSummaryList);
    }
}
