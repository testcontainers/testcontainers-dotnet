namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class HealthCheckFixture : IImage, IAsyncLifetime
  {
    private readonly IFutureDockerImage _image = new ImageFromDockerfileBuilder()
      .WithDockerfileDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "healthWaitStrategy"))
      .Build();

    public string Repository => _image.Repository;

    public string Registry => _image.Registry;

    public string Tag => _image.Tag;

    public string Digest => _image.Digest;

    public string FullName => _image.FullName;

    public string GetHostname()
    {
      return _image.GetHostname();
    }

    public bool MatchLatestOrNightly()
    {
      return _image.MatchLatestOrNightly();
    }

    public bool MatchVersion(Predicate<string> predicate)
    {
      return _image.MatchVersion(predicate);
    }

    public bool MatchVersion(Predicate<Version> predicate)
    {
      return _image.MatchVersion(predicate);
    }

    public Task InitializeAsync()
    {
      return _image.CreateAsync();
    }

    public Task DisposeAsync()
    {
      return _image.DeleteAsync();
    }
  }
}
