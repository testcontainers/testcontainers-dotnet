namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class LocalStackTestcontainerTest : IClassFixture<LocalStackFixture>
  {
    private readonly LocalStackFixture localStackFixture;

    public LocalStackTestcontainerTest(LocalStackFixture localStackFixture)
    {
      this.localStackFixture = localStackFixture;
    }

    [Fact]
    public async Task ExecSqsCommandInRunningContainer()
    {
      var execResult = await this.localStackFixture.Container.ExecAsync(new[] { "awslocal", "sqs", "create-queue", "--queue-name", "sample-queue" })
        .ConfigureAwait(false);

      Assert.Contains("http://localhost:4566/000000000000/sample-queue", execResult.Stdout);
    }
  }
}
