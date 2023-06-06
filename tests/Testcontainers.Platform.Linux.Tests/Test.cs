namespace Testcontainers.Tests;

using System.IO;
using ICSharpCode.SharpZipLib.Tar;

public sealed class Test : IAsyncLifetime
{
    private static readonly IOperatingSystem OS = new Unix(dockerEndpointAuthConfig: null);

    private readonly IContainer _container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint(CommonCommands.SleepInfinity)
        .Build();

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    [Theory]
    public async Task CopyDirectory(string source)
    {
        await _container.CopyAsync(new DirectoryInfo(source), "/tmp/foo");
    }

    private sealed class TarOutputMemoryStream : MemoryStream
    {
        private readonly string _rootDirectoryPath;

        private readonly string _sourceFilePath;

        private readonly string _targetFilePath;

        public TarOutputMemoryStream(FileInfo fileSystemInfo, string targetFilePath)
            : this(fileSystemInfo.Directory, targetFilePath)
        {
            _sourceFilePath = fileSystemInfo.FullName;
        }

        public TarOutputMemoryStream(FileSystemInfo fileSystemInfo, string targetFilePath)
            : this(fileSystemInfo.FullName)
        {
            _sourceFilePath = fileSystemInfo.FullName;
            _targetFilePath = targetFilePath;
        }

        private TarOutputMemoryStream(string rootDirectoryPath)
        {
            _rootDirectoryPath = OS.NormalizePath(rootDirectoryPath.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        public async Task TarAsync()
        {
            using (var tarOutputStream = new TarOutputStream(this, Encoding.Default))
            {
                tarOutputStream.IsStreamOwner = false;

                foreach (var file in Directory.GetFiles(_sourceFilePath))
                {
                    using (var inputStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var relativeFilePath = file.Substring(_sourceFilePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length + 1);
                        relativeFilePath = OS.NormalizePath(Path.Combine(_targetFilePath, relativeFilePath));

                        var entry = TarEntry.CreateTarEntry(relativeFilePath);
                        entry.Size = inputStream.Length;

                        await tarOutputStream.PutNextEntryAsync(entry, default)
                            .ConfigureAwait(false);

                        await inputStream.CopyToAsync(tarOutputStream, 4096, default)
                            .ConfigureAwait(false);

                        await tarOutputStream.CloseEntryAsync(default)
                            .ConfigureAwait(false);
                    }
                }
            }

            Seek(0, SeekOrigin.Begin);
        }
    }
}