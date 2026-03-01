namespace Testcontainers.Seq;

public sealed partial class DeclineLicenseAgreementTest
{
    [GeneratedRegex("The image '.+' requires you to accept a license agreement\\.")]
    private static partial Regex LicenseAgreementNotAccepted();

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void WithoutAcceptingLicenseAgreementThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new SeqBuilder(TestSession.GetImageFromDockerfile()).Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void WithLicenseAgreementDeclinedThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new SeqBuilder(TestSession.GetImageFromDockerfile()).WithAcceptLicenseAgreement(false).Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }
}