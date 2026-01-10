namespace Testcontainers.Tests;

public abstract class TarOutputMemoryStreamTest : FileTestBase
{
    private const string TargetDirectoryPath = "/tmp";

    private readonly TarOutputMemoryStream _tarOutputMemoryStream = new TarOutputMemoryStream(TargetDirectoryPath, NullLogger.Instance);

    protected TarOutputMemoryStreamTest() : base()
    {
    }

    protected override void DisposeManagedResources()
    {
        base.DisposeManagedResources();

        _tarOutputMemoryStream.Dispose();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void TestFileExistsInTarFile()
    {
        // Given
        var actual = new List<string>();

        _tarOutputMemoryStream.Close();
        _tarOutputMemoryStream.Seek(0, SeekOrigin.Begin);

        // When
        using var tarIn = TarArchive.CreateInputTarArchive(_tarOutputMemoryStream, Encoding.Default);
        tarIn.ProgressMessageEvent += (_, entry, _) => actual.Add(entry.Name);
        tarIn.ListContents();

        // Then
        Assert.Contains(actual, file => file.EndsWith(_testFile.Name));
    }

    [UsedImplicitly]
    public sealed class FromResourceMapping : TarOutputMemoryStreamTest, IResourceMapping, IClassFixture<FromResourceMapping.HttpFixture>, IAsyncLifetime
    {
        private readonly string _testHttpUri;

        private readonly string _testFileUri;

        public FromResourceMapping(HttpFixture httpFixture)
        {
            _testHttpUri = httpFixture.BaseAddress;
            _testFileUri = new Uri(_testFile.FullName).ToString();
        }

        public MountType Type
            => MountType.Bind;

        public AccessMode AccessMode
            => AccessMode.ReadOnly;

        public string Source
            => string.Empty;

        public string Target
            => string.Join("/", TargetDirectoryPath, _testFile.Name);

        public uint UserId
            => 0;

        public uint GroupId
            => 0;

        public UnixFileModes FileMode
            => Unix.FileMode644;

        public async ValueTask InitializeAsync()
        {
            await _tarOutputMemoryStream.AddAsync(this)
                .ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        Task IFutureResource.CreateAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        Task IFutureResource.DeleteAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
        {
            return File.ReadAllBytesAsync(_testFile.FullName, ct);
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task TestFileExistsInContainer()
        {
            // Given
            var targetFilePath1 = string.Join("/", string.Empty, "tmp", Guid.NewGuid(), _testFile.Name);
            var targetFilePath2 = string.Join("/", string.Empty, "tmp", Guid.NewGuid(), _testFile.Name);
            var targetFilePath3 = string.Join("/", string.Empty, "tmp", Guid.NewGuid(), _testFile.Name);
            var targetDirectoryPath1 = string.Join("/", string.Empty, "tmp", Guid.NewGuid());
            var targetDirectoryPath2 = string.Join("/", string.Empty, "tmp", Guid.NewGuid());
            var targetDirectoryPath3 = string.Join("/", string.Empty, "tmp", Guid.NewGuid());
            var targetDirectoryPath4 = string.Join("/", string.Empty, "tmp", Guid.NewGuid());
            var targetDirectoryPath5 = string.Join("/", string.Empty, "tmp", Guid.NewGuid());

            var targetFilePaths = new List<string>();
            targetFilePaths.Add(targetFilePath1);
            targetFilePaths.Add(targetFilePath2);
            targetFilePaths.Add(targetFilePath3);
            targetFilePaths.Add(string.Join("/", targetDirectoryPath1, _testFile.Name));
            targetFilePaths.Add(string.Join("/", targetDirectoryPath2, _testFile.Name));
            targetFilePaths.Add(string.Join("/", targetDirectoryPath3, _testFile.Name));
            targetFilePaths.Add(string.Join("/", targetDirectoryPath4, _testFile.Name));
            targetFilePaths.Add(string.Join("/", targetDirectoryPath5, _testFile.Name));

            await using var container = new ContainerBuilder(CommonImages.Alpine)
                .WithEntrypoint(CommonCommands.SleepInfinity)
                .WithResourceMapping(_testFile, new FileInfo(targetFilePath1))
                .WithResourceMapping(_testFile.FullName, targetDirectoryPath1)
                .WithResourceMapping(_testFile.Directory!.FullName, targetDirectoryPath2)
                .WithResourceMapping(_testHttpUri, targetFilePath2)
                .WithResourceMapping(_testFileUri, targetDirectoryPath3)
                .Build();

            // When
            var fileContent = await GetAllBytesAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            await container.StartAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            await container.CopyAsync(fileContent, targetFilePath3, ct: TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            await container.CopyAsync(_testFile.FullName, targetDirectoryPath4, ct: TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            await container.CopyAsync(_testFile.Directory!.FullName, targetDirectoryPath5, ct: TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            var execResults = await Task.WhenAll(targetFilePaths.Select(containerFilePath => container.ExecAsync(new[] { "test", "-f", containerFilePath }, TestContext.Current.CancellationToken)))
                .ConfigureAwait(true);

            Assert.All(execResults, result => Assert.Equal(0, result.ExitCode));
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task TestFileHasExpectedOwnerGroupMode()
        {
            // Given
            const uint nobody = 65534;
            const uint uid = nobody;
            const uint gid = nobody;
            const UnixFileModes mode = Unix.FileMode755;

            var modeOctal = Convert.ToString((int)mode, 8).PadLeft(4, '0');
            var expected = uid + ":" + gid + " " + modeOctal;

            var resourceContent = Array.Empty<byte>();

            var targetFilePath1 = string.Join("/", string.Empty, "tmp", Guid.NewGuid(), _testFile.Name);
            var targetFilePath2 = string.Join("/", string.Empty, "tmp", Guid.NewGuid(), _testFile.Name);

            var targetFilePaths = new List<string>();
            targetFilePaths.Add(targetFilePath1);
            targetFilePaths.Add(targetFilePath2);

            await using var container = new ContainerBuilder(CommonImages.Alpine)
                .WithEntrypoint(CommonCommands.SleepInfinity)
                .WithResourceMapping(resourceContent, targetFilePath1, uid, gid, mode)
                .Build();

            // When
            await container.StartAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            await container.CopyAsync(resourceContent, targetFilePath2, uid, gid, mode, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            var execResults = await Task.WhenAll(targetFilePaths.Select(containerFilePath => container.ExecAsync(new[] { "stat", "-c", "%u:%g %04a", containerFilePath }, TestContext.Current.CancellationToken)))
                .ConfigureAwait(true);

            Assert.All(execResults, result => Assert.Equal(expected, result.Stdout.Trim()));
        }

        [UsedImplicitly]
        public sealed class HttpFixture : IAsyncLifetime
        {
            private const ushort HttpPort = 80;

            private readonly IContainer _container = new ContainerBuilder(CommonImages.Socat)
                .WithCommand("-v")
                .WithCommand($"TCP-LISTEN:{HttpPort},crlf,reuseaddr,fork")
                .WithCommand("SYSTEM:'echo -e \"HTTP/1.1 200 OK\\nContent-Length: 0\\n\\n\"'")
                .WithPortBinding(HttpPort, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request))
                .Build();

            public string BaseAddress
                => new UriBuilder(Uri.UriSchemeHttp, _container.Hostname, _container.GetMappedPublicPort(HttpPort)).ToString();

            public ValueTask InitializeAsync()
            {
                return new ValueTask(_container.StartAsync());
            }

            public ValueTask DisposeAsync()
            {
                return _container.DisposeAsync();
            }
        }
    }

    [UsedImplicitly]
    public sealed class FromFile : TarOutputMemoryStreamTest, IAsyncLifetime
    {
        public async ValueTask InitializeAsync()
        {
            await _tarOutputMemoryStream.AddAsync(_testFile, 0, 0, Unix.FileMode644)
                .ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }

    [UsedImplicitly]
    public sealed class FromDirectory : TarOutputMemoryStreamTest, IAsyncLifetime
    {
        public async ValueTask InitializeAsync()
        {
            await _tarOutputMemoryStream.AddAsync(_testFile.Directory, true, 0, 0, Unix.FileMode644)
                .ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }

    public sealed class UnixFileModeTest
    {
        [Theory]
        [InlineData(Unix.FileMode644, "644")]
        [InlineData(Unix.FileMode666, "666")]
        [InlineData(Unix.FileMode700, "700")]
        [InlineData(Unix.FileMode755, "755")]
        [InlineData(Unix.FileMode777, "777")]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public void UnixFileModeResolvesToPosixFilePermission(UnixFileModes fileMode, string posixFilePermission)
        {
            Assert.Equal(Convert.ToInt32(posixFilePermission, 8), Convert.ToInt32(fileMode));
        }
    }
}