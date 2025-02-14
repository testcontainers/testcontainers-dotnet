namespace Testcontainers.Db2;

public sealed partial class DeclineLicenseAgreementTest
{
    [GeneratedRegex("The image '.+' requires you to accept a license agreement\\.")]
    private static partial Regex LicenseAgreementNotAccepted();

    [Fact]
    public void WithoutAcceptingLicenseAgreementThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new Db2Builder().Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }

    [Fact]
    public void WithLicenseAgreementDeclinedThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new Db2Builder().WithAcceptLicenseAgreement(false).Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }
}