namespace Testcontainers.Neo4j;

public sealed class Neo4jBuilderTest
{
    [Theory]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [InlineData("neo4j:5.23.0", "5.23.0-enterprise")]
    [InlineData("neo4j:5.23", "5.23-enterprise")]
    [InlineData("neo4j:5", "5-enterprise")]
    [InlineData("neo4j:5.23.0-community", "5.23.0-community")]
    [InlineData("neo4j:5.23-community", "5.23-community")]
    [InlineData("neo4j:5-community", "5-community")]
    [InlineData("neo4j:community", "community")]
    [InlineData("neo4j:5.23.0-bullseye", "5.23.0-enterprise-bullseye")]
    [InlineData("neo4j:5.23-bullseye", "5.23-enterprise-bullseye")]
    [InlineData("neo4j:5-bullseye", "5-enterprise-bullseye")]
    [InlineData("neo4j:bullseye", "enterprise-bullseye")]
    [InlineData("neo4j:5.23.0-enterprise-bullseye", "5.23.0-enterprise-bullseye")]
    [InlineData("neo4j:5.23-enterprise-bullseye", "5.23-enterprise-bullseye")]
    [InlineData("neo4j:5-enterprise-bullseye", "5-enterprise-bullseye")]
    [InlineData("neo4j:enterprise-bullseye", "enterprise-bullseye")]
    [InlineData("neo4j:5.23.0-enterprise", "5.23.0-enterprise")]
    [InlineData("neo4j:5.23-enterprise", "5.23-enterprise")]
    [InlineData("neo4j:5-enterprise", "5-enterprise")]
    [InlineData("neo4j:enterprise", "enterprise")]
    [InlineData("neo4j", "enterprise")]
    [InlineData("neo4j@sha256:20eb19e3d60f9f07c12c89eac8d8722e393be7e45c6d7e56004a2c493b8e2032", null)]
    public void AppendsEnterpriseSuffixWhenEnterpriseEditionLicenseAgreementIsAccepted(string image, string expected)
    {
        var neo4jContainer = new Neo4jBuilder().WithImage(image).WithEnterpriseEdition(true).Build();
        Assert.Equal(expected, neo4jContainer.Image.Tag);
    }

    [Theory]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [ClassData(typeof(Neo4jBuilderConfigurations))]
    public void ThrowsArgumentExceptionWhenEnterpriseEditionLicenseAgreementIsNotAccepted(Neo4jBuilder neo4jBuilder)
    {
        Assert.Throws<ArgumentException>(neo4jBuilder.Build);
    }

    private sealed class Neo4jBuilderConfigurations : TheoryData<Neo4jBuilder>
    {
        public Neo4jBuilderConfigurations()
        {
            Add(new Neo4jBuilder().WithImage(Neo4jBuilder.Neo4jImage + "-enterprise"));
            Add(new Neo4jBuilder().WithEnterpriseEdition(false));
        }
    }
}