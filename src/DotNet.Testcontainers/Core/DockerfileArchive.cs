namespace DotNet.Testcontainers.Core
{
  using System;
  using System.IO;
  using System.Linq;
  using ICSharpCode.SharpZipLib.Tar;

  internal sealed class DockerfileArchive : ITarArchive
  {
    private static readonly char[] TrimLeadingChars = { '/' };

    public DockerfileArchive(string baseDirectory) : this(new DirectoryInfo(baseDirectory))
    {
    }

    public DockerfileArchive(DirectoryInfo baseDirectory)
    {
      this.BaseDirectory = baseDirectory;

      this.DockerfileArchiveFile = new FileInfo($"{Path.GetTempPath()}/Dockerfile.tar");

      if (!this.BaseDirectory.Exists)
      {
        throw new ArgumentException($"Directory '{this.BaseDirectory.FullName}' does not exist.");
      }

      if (!this.BaseDirectory.GetFiles().Any(file => "Dockerfile".Equals(file.Name)))
      {
        throw new ArgumentException($"Dockerfile does not exist in '{this.BaseDirectory.FullName}'.");
      }
    }

    public DirectoryInfo BaseDirectory { get; }

    public FileInfo DockerfileArchiveFile { get; }

    public string Tar()
    {
      using (var dockerfileArchiveStream = File.Create(this.DockerfileArchiveFile.FullName))
      {
        using (var dockerfileArchive = TarArchive.CreateOutputTarArchive(dockerfileArchiveStream))
        {
          dockerfileArchive.RootPath = this.BaseDirectory.FullName;

          void Tar(string baseDirectory)
          {
            baseDirectory = baseDirectory.Replace('\\', '/');

            void WriteEntry(string entry)
            {
              entry = entry.Replace('\\', '/');

              var tarEntry = TarEntry.CreateEntryFromFile(entry);
              tarEntry.Name = entry.Replace(dockerfileArchive.RootPath, string.Empty).TrimStart(TrimLeadingChars);
              dockerfileArchive.WriteEntry(tarEntry, File.Exists(entry));
            }

            if (!dockerfileArchive.RootPath.Equals(baseDirectory))
            {
              WriteEntry(baseDirectory);
            }

            Directory.GetFiles(baseDirectory).ToList().ForEach(WriteEntry);

            Directory.GetDirectories(baseDirectory).ToList().ForEach(Tar);
          }

          Tar(this.BaseDirectory.FullName);
        }
      }

      return this.DockerfileArchiveFile.FullName;
    }
  }
}
