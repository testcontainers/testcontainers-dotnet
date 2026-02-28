namespace Testcontainers.Db2;

public sealed partial class DeclineLicenseAgreementTest
{
    [GeneratedRegex("The image '.+' requires you to accept a license agreement\\.")]
    private static partial Regex LicenseAgreementNotAccepted();

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void WithoutAcceptingLicenseAgreementThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new Db2Builder(TestSession.GetImageFromDockerfile()).Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void WithLicenseAgreementDeclinedThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new Db2Builder(TestSession.GetImageFromDockerfile()).WithAcceptLicenseAgreement(false).Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }
}