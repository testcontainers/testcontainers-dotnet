namespace DotNet.Testcontainers.Tests.Unit
{
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class TestcontainersVolumeTest : IClassFixture<VolumeFixture>, IAsyncLifetime
  {
    private const string Destination = "/tmp/";

    private const string TmpfsDestination = "/dev/shm";

    private readonly ITestcontainersContainer testcontainer1;

    private readonly ITestcontainersContainer testcontainer2;

    public TestcontainersVolumeTest(VolumeFixture volumeFixture)
    {
      var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand("touch /tmp/$(uname -n) && tail -f /dev/null")
        .WithVolumeMount(volumeFixture.Volume.Name, Destination)
        .WithTmpfsMount(TmpfsDestination);

      this.testcontainer1 = testcontainersBuilder
        .WithHostname(nameof(this.testcontainer1))
        .Build();

      this.testcontainer2 = testcontainersBuilder
        .WithHostname(nameof(this.testcontainer2))
        .Build();
    }

    public Task InitializeAsync()
    {
      return Task.WhenAll(this.testcontainer1.StartAsync(), this.testcontainer2.StartAsync());
    }

    public Task DisposeAsync()
    {
      return Task.WhenAll(this.testcontainer1.DisposeAsync().AsTask(), this.testcontainer2.DisposeAsync().AsTask());
    }

    [Fact]
    public async Task WithVolumeMount()
    {
      Assert.Equal(0, (await this.testcontainer1.ExecAsync(new[] { "test", "-f", Path.Combine(Destination, nameof(this.testcontainer2)) })).ExitCode);
      Assert.Equal(0, (await this.testcontainer2.ExecAsync(new[] { "test", "-f", Path.Combine(Destination, nameof(this.testcontainer1)) })).ExitCode);
    }

    [Fact]
    public async Task WithTmpfsVolumeMount()
    {
      Assert.Equal(0, (await this.testcontainer1.ExecAsync(new[] { "test", "-d", TmpfsDestination })).ExitCode);
      Assert.Equal(0, (await this.testcontainer2.ExecAsync(new[] { "test", "-d", TmpfsDestination })).ExitCode);
    }
  }
}
