using DotNet.Testcontainers.Images;
using Xunit;

namespace DotNet.Testcontainers.Tests.Fixtures
{
  public sealed class MatchImageFixture : TheoryData<string, DockerImage>
  {
    public MatchImageFixture()
    {
      Add("test-image", new DockerImage(string.Empty, "test-image"));
      Add("test-image:test-tag", new DockerImage(string.Empty, "test-image", "test-tag"));
      Add("test-repository.com/test-image:test-tag", new DockerImage("test-repository.com", "test-image", "test-tag"));
      Add("test-parent-repository/test-child-repository/test-image:test-tag", new DockerImage("test-parent-repository/test-child-repository", "test-image", "test-tag"));
      Add("test-parent-repository/test-child-repository/test-image@sha256:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", new DockerImage("test-parent-repository/test-child-repository", "test-image", "@sha256:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
      Add("test-parent-repository/test-child-repository/test-image:test-tag@sha256:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", new DockerImage("test-parent-repository/test-child-repository", "test-image", "test-tag@sha256:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
    }
  }
}
