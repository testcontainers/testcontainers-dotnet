using System;

namespace Testcontainers.Neo4j;

public sealed class Neo4jBuilderConfigurationTest
{
    [Fact]
    public void CreatingEnterpriseContainerWithoutLicenseAgreementShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new Neo4jBuilder()
            .WithImage(Neo4jBuilder.Neo4jEnterpriseImage)
            .Build());
    }
}