namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using Xunit;

  public class GitHub
  {
    [Fact]
    public Task PullRequest361()
    {
      Assert.NotEqual(Guid.Empty, new TestcontainersSession().Id);
      return Task.CompletedTask;
    }
  }
}
