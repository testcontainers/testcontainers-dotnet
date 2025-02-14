namespace Testcontainers.ServiceBus;

public sealed partial class DeclineLicenseAgreementTest
{
    [GeneratedRegex("The image '.+' requires you to accept a license agreement\\.")]
    private static partial Regex LicenseAgreementNotAccepted();

    [Fact]
    public void WithoutAcceptingLicenseAgreementThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new ServiceBusBuilder().Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }

    [Fact]
    public void WithLicenseAgreementDeclinedThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new ServiceBusBuilder().WithAcceptLicenseAgreement(false).Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }
}