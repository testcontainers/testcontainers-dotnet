namespace Testcontainers.Tests;

public abstract class TarOutputMemoryStreamTest
{
    protected const string TargetDirectoryPath = "/tmp";

    protected TarOutputMemoryStreamTest()
    {
        using var fileStream = TestFile.Create();
        fileStream.WriteByte(13);
    }

    protected TarOutputMemoryStream TarOutputMemoryStream { get; }
        = new TarOutputMemoryStream(TargetDirectoryPath);

    protected FileInfo TestFile { get; }
        = new FileInfo(Path.Combine(TestSession.TempDirectoryPath, Path.GetRandomFileName()));

    [Fact]
    public Task TarFileContainsTestFile()
    {
        return Task.CompletedTask;
    }

    [UsedImplicitly]
    public sealed class FromResourceMapping : TarOutputMemoryStreamTest, IResourceMapping, IAsyncLifetime, IDisposable
    {
        public MountType Type
            => MountType.Bind;

        public AccessMode AccessMode
            => AccessMode.ReadOnly;

        public string Source
            => string.Empty;

        public string Target
            => string.Join("/", TargetDirectoryPath, TestFile.Name);

        public UnixFileMode FileMode
            => Unix.FileMode644;

        public Task InitializeAsync()
        {
            return TarOutputMemoryStream.AddAsync(this);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            TarOutputMemoryStream.Dispose();
        }

        public Task CreateAsync(CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
        {
            return File.ReadAllBytesAsync(TestFile.FullName, ct);
        }
    }

    [UsedImplicitly]
    public sealed class FromFile : TarOutputMemoryStreamTest, IAsyncLifetime, IDisposable
    {
        public Task InitializeAsync()
        {
            return TarOutputMemoryStream.AddAsync(TestFile, Unix.FileMode644);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            TarOutputMemoryStream.Dispose();
        }
    }

    [UsedImplicitly]
    public sealed class FromDirectory : TarOutputMemoryStreamTest, IAsyncLifetime, IDisposable
    {
        public Task InitializeAsync()
        {
            return TarOutputMemoryStream.AddAsync(TestFile.Directory, true, Unix.FileMode644);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            TarOutputMemoryStream.Dispose();
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
        public void UnixFileModeResolvesToPosixFilePermission(UnixFileMode fileMode, string posixFilePermission)
        {
            Assert.Equal(Convert.ToInt32(posixFilePermission, 8), Convert.ToInt32(fileMode));
        }
    }
}