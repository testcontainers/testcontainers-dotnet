namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class HealthCheckFixture : IImage, IAsyncLifetime
  {
    private readonly IFutureDockerImage image = new ImageFromDockerfileBuilder()
      .WithDockerfileDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "healthWaitStrategy"))
      .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
      .Build();

    public string Repository => this.image.Repository;

    public string Name => this.image.Name;

    public string Tag => this.image.Tag;

    public string FullName => this.image.FullName;

    public string GetHostname()
    {
      return this.image.GetHostname();
    }

    public Task InitializeAsync()
    {
      return this.image.CreateAsync();
    }

    public Task DisposeAsync()
    {
      return this.image.DeleteAsync();
    }
  }
}
