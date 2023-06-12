namespace Testcontainers.Tests;

using System.IO;
using ICSharpCode.SharpZipLib.Tar;

public sealed class Archive : MemoryStream
{
  private static readonly IOperatingSystem OS = new Unix(dockerEndpointAuthConfig: null);

  private readonly TarOutputStream _tarOutputStream;

  private readonly string _targetDirectoryPath;

  public Archive(string targetDirectoryPath)
  {
    _tarOutputStream = new TarOutputStream(this, Encoding.Default);
    _tarOutputStream.IsStreamOwner = false;
    _targetDirectoryPath = targetDirectoryPath;
  }

  public async Task AddAsync(FileInfo file)
  {
    using (var inputStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
    {
      var targetFilePath = Path.Combine(_targetDirectoryPath, file.Name);

      var tarEntry = TarEntry.CreateTarEntry(OS.NormalizePath(targetFilePath));
      tarEntry.Size = inputStream.Length;

      await AddAsync(tarEntry, inputStream)
        .ConfigureAwait(false);
    }
  }

  public Task AddAsync(DirectoryInfo directory)
  {
    return AddAsync(directory, true);
  }

  public async Task AddAsync(DirectoryInfo directory, bool recurse)
  {
    foreach (var file in directory.GetFiles())
    {
      using (var inputStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
      {
        // TODO: Only substring root path
        var targetFilePath = Path.Combine(_targetDirectoryPath, file.FullName.Substring(directory.FullName.Length + 1));

        var tarEntry = TarEntry.CreateTarEntry(OS.NormalizePath(targetFilePath));
        tarEntry.Size = inputStream.Length;

        await AddAsync(tarEntry, inputStream)
          .ConfigureAwait(false);
      }
    }

    foreach (var d in directory.GetDirectories())
    {
      await AddAsync(d, recurse)
        .ConfigureAwait(false);
    }
  }

  public Task AddAsync(IResourceMapping resourceMapping)
  {
    return Task.CompletedTask;
  }

  public Task TarAsync()
  {
    _tarOutputStream.Close();
    Seek(0, SeekOrigin.Begin);
    return Task.CompletedTask;
  }

  private async Task AddAsync(TarEntry tarEntry, Stream stream, CancellationToken ct = default)
  {
    const int defaultCopyBufferSize = 81920;

    await _tarOutputStream.PutNextEntryAsync(tarEntry, ct)
      .ConfigureAwait(false);

    await stream.CopyToAsync(_tarOutputStream, defaultCopyBufferSize, ct)
      .ConfigureAwait(false);

    await _tarOutputStream.CloseEntryAsync(ct)
      .ConfigureAwait(false);
  }
}

public sealed class TarOutputMemoryStreamTest
{
  [Fact(Skip = "Do not run on CI")]
  public async Task Test()
  {
    using var tarball = new Archive("/tmp/foo/bar/baz");
    await tarball.AddAsync(new FileInfo(string.Empty));
    await tarball.AddAsync(new DirectoryInfo(string.Empty));
    await tarball.TarAsync();
    using var fs = File.Open(Path.Combine("C:", "Temp", Guid.NewGuid().ToString("D")) + ".tar", FileMode.Create);
    await tarball.CopyToAsync(fs);
  }
}