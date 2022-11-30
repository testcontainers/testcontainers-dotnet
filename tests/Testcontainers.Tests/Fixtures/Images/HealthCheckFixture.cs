namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class HealthCheckFixture : IDockerImage, IAsyncLifetime
  {
    private readonly IDockerImage image = new DockerImage(string.Empty, Guid.NewGuid().ToString("D"), DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));

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
      return new ImageFromDockerfileBuilder()
        .WithName(this)
        .WithDockerfileDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "healthWaitStrategy"))
        .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
        .Build();
    }

    public Task DisposeAsync()
    {
      return Task.CompletedTask;
    }
  }
}
