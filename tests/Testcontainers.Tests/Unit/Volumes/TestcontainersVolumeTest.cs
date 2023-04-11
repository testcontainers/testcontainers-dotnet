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

    private readonly IContainer _testcontainer1;

    private readonly IContainer _testcontainer2;

    public TestcontainersVolumeTest(VolumeFixture volumeFixture)
    {
      var testcontainersBuilder = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand("touch /tmp/$(uname -n) && tail -f /dev/null")
        .WithVolumeMount(volumeFixture.Volume.Name, Destination)
        .WithTmpfsMount(TmpfsDestination);

      _testcontainer1 = testcontainersBuilder
        .WithHostname(nameof(_testcontainer1))
        .Build();

      _testcontainer2 = testcontainersBuilder
        .WithHostname(nameof(_testcontainer2))
        .Build();
    }

    public Task InitializeAsync()
    {
      return Task.WhenAll(_testcontainer1.StartAsync(), _testcontainer2.StartAsync());
    }

    public Task DisposeAsync()
    {
      return Task.WhenAll(_testcontainer1.DisposeAsync().AsTask(), _testcontainer2.DisposeAsync().AsTask());
    }

    [Fact]
    public async Task WithVolumeMount()
    {
      Assert.Equal(0, (await _testcontainer1.ExecAsync(new[] { "test", "-f", Path.Combine(Destination, nameof(_testcontainer2)) })).ExitCode);
      Assert.Equal(0, (await _testcontainer2.ExecAsync(new[] { "test", "-f", Path.Combine(Destination, nameof(_testcontainer1)) })).ExitCode);
    }

    [Fact]
    public async Task WithTmpfsVolumeMount()
    {
      Assert.Equal(0, (await _testcontainer1.ExecAsync(new[] { "test", "-d", TmpfsDestination })).ExitCode);
      Assert.Equal(0, (await _testcontainer2.ExecAsync(new[] { "test", "-d", TmpfsDestination })).ExitCode);
    }
  }
}
