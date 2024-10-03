namespace Testcontainers.Neo4j;

public sealed class Neo4jBuilderTest
{
    [Theory]
    [ClassData(typeof(Neo4jBuilderConfigurations))]
    public void ThrowsArgumentExceptionWhenEnterpriseEditionLicenseAgreementIsNotAccepted(Neo4jBuilder neo4jBuilder)
    {
        Assert.Throws<ArgumentException>(neo4jBuilder.Build);
    }

    private sealed class Neo4jBuilderConfigurations : TheoryData<Neo4jBuilder>
    {
        public Neo4jBuilderConfigurations()
        {
            Add(new Neo4jBuilder().WithImage(Neo4jBuilder.Neo4jEnterpriseImage));
            Add(new Neo4jBuilder().WithEnterpriseEdition(false));
        }
    }
}