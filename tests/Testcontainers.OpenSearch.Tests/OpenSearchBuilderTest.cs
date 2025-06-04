namespace Testcontainers.OpenSearch;

public sealed class OpenSearchBuilderTest
{
    [Theory]
    [InlineData("opensearchproject/opensearch:1.0.0")]
    [InlineData("opensearchproject/opensearch:1.1.0")]
    [InlineData("opensearchproject/opensearch:2.11.0")]
    [InlineData("opensearchproject/opensearch:2.11.1")]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ShouldUseHardcodedDefaultPassword(string image)
    {
        // Given
        var opensearchContainer = new OpenSearchBuilder().WithImage(image).Build();

        // When
        var credentials = opensearchContainer.GetCredentials();

        // Then
        Assert.Equal("admin", credentials.Password);
    }
}