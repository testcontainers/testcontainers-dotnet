namespace Testcontainers.Tests;

public abstract class TarOutputMemoryStreamTest
{
    private const string TargetDirectoryPath = "/tmp";

    private readonly TarOutputMemoryStream _tarOutputMemoryStream = new TarOutputMemoryStream(TargetDirectoryPath, NullLogger.Instance);

    private readonly FileInfo _testFile = new FileInfo(Path.Combine(TestSession.TempDirectoryPath, Path.GetRandomFileName()));

    protected TarOutputMemoryStreamTest()
    {
        using var fileStream = _testFile.Open(FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        fileStream.WriteByte(13);
    }

    [Fact]
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
    public sealed class FromResourceMapping : TarOutputMemoryStreamTest, IResourceMapping, IClassFixture<FromResourceMapping.HttpFixture>, IAsyncLifetime, IDisposable
    {
        private readonly string _testHttpUri;

        private readonly string _testFileUri;

        public FromResourceMapping(FromResourceMapping.HttpFixture httpFixture)
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

        public UnixFileModes FileMode
            => Unix.FileMode644;

        public Task InitializeAsync()
        {
            return _tarOutputMemoryStream.AddAsync(this);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _tarOutputMemoryStream.Dispose();
            _testFile.Delete();
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

            await using var container = new ContainerBuilder()
                .WithImage(CommonImages.Alpine)
                .WithEntrypoint(CommonCommands.SleepInfinity)
                .WithResourceMapping(_testFile, new FileInfo(targetFilePath1))
                .WithResourceMapping(_testFile.FullName, targetDirectoryPath1)
                .WithResourceMapping(_testFile.Directory.FullName, targetDirectoryPath2)
                .WithResourceMapping(_testHttpUri, targetFilePath2)
                .WithResourceMapping(_testFileUri, targetDirectoryPath3)
                .Build();

            // When
            var fileContent = await GetAllBytesAsync()
                .ConfigureAwait(true);

            await container.StartAsync()
                .ConfigureAwait(true);

            await container.CopyAsync(fileContent, targetFilePath3)
                .ConfigureAwait(true);

            await container.CopyAsync(_testFile.FullName, targetDirectoryPath4)
                .ConfigureAwait(true);

            await container.CopyAsync(_testFile.Directory.FullName, targetDirectoryPath5)
                .ConfigureAwait(true);

            // Then
            var execResults = await Task.WhenAll(targetFilePaths.Select(containerFilePath => container.ExecAsync(new[] { "test", "-f", containerFilePath })))
                .ConfigureAwait(true);

            Assert.All(execResults, result => Assert.Equal(0, result.ExitCode));
        }

        public sealed class HttpFixture : IAsyncLifetime
        {
            private const ushort HttpPort = 80;

            private readonly IContainer _container = new ContainerBuilder()
                .WithImage(CommonImages.Alpine)
                .WithEntrypoint("/bin/sh", "-c")
                .WithCommand($"while true; do echo \"HTTP/1.1 200 OK\r\n\" | nc -l -p {HttpPort}; done")
                .WithPortBinding(HttpPort, true)
                .Build();

            public string BaseAddress
                => new UriBuilder(Uri.UriSchemeHttp, _container.Hostname, _container.GetMappedPublicPort(HttpPort)).ToString();

            public Task InitializeAsync()
            {
                return _container.StartAsync();
            }

            public Task DisposeAsync()
            {
                return _container.DisposeAsync().AsTask();
            }
        }
    }

    [UsedImplicitly]
    public sealed class FromFile : TarOutputMemoryStreamTest, IAsyncLifetime, IDisposable
    {
        public Task InitializeAsync()
        {
            return _tarOutputMemoryStream.AddAsync(_testFile, Unix.FileMode644);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _tarOutputMemoryStream.Dispose();
            _testFile.Delete();
        }
    }

    [UsedImplicitly]
    public sealed class FromDirectory : TarOutputMemoryStreamTest, IAsyncLifetime, IDisposable
    {
        public Task InitializeAsync()
        {
            return _tarOutputMemoryStream.AddAsync(_testFile.Directory, true, Unix.FileMode644);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _tarOutputMemoryStream.Dispose();
            _testFile.Delete();
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
        public void UnixFileModeResolvesToPosixFilePermission(UnixFileModes fileMode, string posixFilePermission)
        {
            Assert.Equal(Convert.ToInt32(posixFilePermission, 8), Convert.ToInt32(fileMode));
        }
    }
}