namespace Testcontainers.Tests;

public sealed class AcceptLicenseAgreementTest
{
    [Fact]
    public void WithLicenseAgreementAcceptedThrowsArgumentException()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new ContainerBuilder().WithAcceptLicenseAgreement(true).Build());
        Assert.Equal("The module does not require you to accept a license agreement.", exception.Message);
    }
}