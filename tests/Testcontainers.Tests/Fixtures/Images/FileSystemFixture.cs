namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  public class FileSystemFixture : IFileSystem
  {
    public List<string> Lines { get; } = new List<string>();

    public bool Exists { get; set; }

    [CanBeNull]
    public string DirectoryFullName { get; set; }

    public IEnumerable<string> ReadLines(string path)
    {
      return this.Lines;
    }

    public bool FileExists(string directory, string file)
    {
      return this.Exists;
    }

    public string GetDirectoryFullName(string directory)
    {
      return this.DirectoryFullName ?? directory;
    }
  }
}
