using ICSharpCode.SharpZipLib.Core;
using System.Formats.Tar;

namespace Testcontainers.Tests
{
    public class CopyTarArchiveTests : FileTestBase
    {
        public CopyTarArchiveTests() : base()
        {
        }

        [Fact]
        public async Task Should_Copy_TarArchive_ToContainer_BigFile_SystemIO()
        {
            const long fiveMb = 5L * 1024L * 1024L;
            const long fiveGb = fiveMb * 1024L;

            // Given
            using (var fs = new FileStream(_testFile.FullName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.SetLength(fiveMb); // change for fiveGb
                fs.Close();
            }

            var targetDirectoryPath1 = string.Join("/", string.Empty, "tmp", string.Empty);

            var targetFilePaths = new List<string>();
            targetFilePaths.Add(Path.Combine(targetDirectoryPath1, _testFile.Name));

            var bufferFilePath = Path.Combine(_testFile.Directory.Parent.FullName, Path.GetRandomFileName());

            using var memStore = new FileStream(bufferFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            await TarFile.CreateFromDirectoryAsync(_testFile.Directory.FullName, memStore, false, TestContext.Current.CancellationToken);
            await memStore.FlushAsync(TestContext.Current.CancellationToken);
            memStore.Position = 0;

            var container = new ContainerBuilder(CommonImages.Alpine)
                .WithEntrypoint(CommonCommands.SleepInfinity)
                .WithCopyTarArchive(memStore, targetDirectoryPath1)
                .Build();

            // When
            await container.StartAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            var execResults = await Task.WhenAll(targetFilePaths.Select(containerFilePath => container.ExecAsync(new[] { "test", "-f", containerFilePath }, TestContext.Current.CancellationToken)))
                .ConfigureAwait(true);

            Assert.All(execResults, result => Assert.Equal(0, result.ExitCode));

            // add proper cleanup in case of test failure?
            if (File.Exists(bufferFilePath))
                File.Delete(bufferFilePath);
        }

        [Fact]
        public async Task Should_Copy_TarArchive_ToContainer_SystemIO()
        {
            // Given
            var targetDirectoryPath1 = string.Join("/", string.Empty, "tmp", string.Empty);

            var targetFilePaths = new List<string>();
            targetFilePaths.Add(Path.Combine(targetDirectoryPath1, _testFile.Name));

            using var memStore = new MemoryStream(); // the underlying storage for tar archive, can be any Stream.
            await TarFile.CreateFromDirectoryAsync(_testFile.Directory.FullName, memStore, false, TestContext.Current.CancellationToken);
            memStore.Position = 0; // must rewind underlying Stream to start position before copying

            var container = new ContainerBuilder(CommonImages.Alpine)
                .WithEntrypoint(CommonCommands.SleepInfinity)
                .WithCopyTarArchive(memStore, targetDirectoryPath1) // this will copy the Stream with tar archive contents in container just before the container startup
                .Build();

            // When
            await container.StartAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            var execResults = await Task.WhenAll(targetFilePaths.Select(containerFilePath => container.ExecAsync(new[] { "test", "-f", containerFilePath }, TestContext.Current.CancellationToken)))
                .ConfigureAwait(true);

            Assert.All(execResults, result => Assert.Equal(0, result.ExitCode));
        }

        [Fact]
        public async Task Should_Copy_TarArchive_ToContainer_SharpZipLib()
        {
            static string ToTarArchivePath(string s)
            {
                return PathUtils.DropPathRoot(s).Replace(Path.DirectorySeparatorChar, '/');
            }

            // Given
            var targetFilePath1 = string.Join("/", string.Empty, "tmp", Guid.NewGuid(), _testFile.Name);
            var targetFilePath2 = string.Join("/", string.Empty, "tmp", Guid.NewGuid(), _testFile.Name);

            var targetFilePaths = new List<string>();
            targetFilePaths.Add(targetFilePath1);
            targetFilePaths.Add(targetFilePath2);
            if (OperatingSystem.IsWindows())
            {
                targetFilePaths.Add(ToTarArchivePath(_testFile.FullName));
            }

            using var memStore = new MemoryStream(); // the underlying storage for tar archive, can be any Stream.
            using var tarArchive = TarArchive.CreateOutputTarArchive(memStore, Encoding.UTF8);
            tarArchive.IsStreamOwner = false; // setting this property to false is required for copying underlying Stream into container later.

            // entry #1 from the file on host disk, using full path to file
            var entry1 = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateEntryFromFile(_testFile.FullName);
            // explicitly set a path for file in container to targetFilePath1
            entry1.Name = targetFilePath1;
            tarArchive.WriteEntry(entry1, false);

            // entry #2
            var entry2 = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateEntryFromFile(_testFile.FullName);
            entry2.Name = targetFilePath2;
            // custom userid:groupid
            entry2.UserId = 1000;
            entry2.GroupId = 1000;
            tarArchive.WriteEntry(entry2, false);

            if (OperatingSystem.IsWindows())
            {
                // entry #3
                var entry3 = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateEntryFromFile(_testFile.FullName);
                // on Windows entry5.Name will be without C:\\ prefix
                tarArchive.WriteEntry(entry3, false);
            }

            // close the TarArchive, forcing it to write all neccessary data as bytes into underlying Stream
            tarArchive.Close();
            memStore.Position = 0; // must rewind underlying Stream to start position before copying

            var container = new ContainerBuilder(CommonImages.Alpine)
                .WithEntrypoint(CommonCommands.SleepInfinity)
                .WithCopyTarArchive(memStore) // this will copy the Stream with tar archive contents in container just before the container startup
                .Build();

            // When
            await container.StartAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            var execResults = await Task.WhenAll(targetFilePaths.Select(containerFilePath => container.ExecAsync(new[] { "test", "-f", containerFilePath }, TestContext.Current.CancellationToken)))
                .ConfigureAwait(true);

            Assert.All(execResults, result => Assert.Equal(0, result.ExitCode));

            // check that uid is correct
            var result = await container.ExecAsync(new[] { "stat", "-c", "'%u'", targetFilePath2 }, TestContext.Current.CancellationToken);
            Assert.Equal("'1000'\n", result.Stdout);
        }
    }
}
