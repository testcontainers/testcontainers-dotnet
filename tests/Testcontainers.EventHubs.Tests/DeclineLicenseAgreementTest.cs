namespace Testcontainers.EventHubs;

public sealed partial class DeclineLicenseAgreementTest
{
    private const string EventHubsName = "eh-1";

    private const string EventHubsConsumerGroupName = "cg-1";

    [GeneratedRegex("The image '.+' requires you to accept a license agreement\\.")]
    private static partial Regex LicenseAgreementNotAccepted();

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void WithoutAcceptingLicenseAgreementThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new EventHubsBuilder(TestSession.GetImageFromDockerfile()).WithConfigurationBuilder(GetServiceConfiguration()).Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void WithLicenseAgreementDeclinedThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new EventHubsBuilder(TestSession.GetImageFromDockerfile()).WithAcceptLicenseAgreement(false).WithConfigurationBuilder(GetServiceConfiguration()).Build());
        Assert.Matches(LicenseAgreementNotAccepted(), exception.Message);
    }

    private static EventHubsServiceConfiguration GetServiceConfiguration()
    {
        return EventHubsServiceConfiguration.Create().WithEntity(EventHubsName, 2, EventHubConsumerClient.DefaultConsumerGroupName, EventHubsConsumerGroupName);
    }
}