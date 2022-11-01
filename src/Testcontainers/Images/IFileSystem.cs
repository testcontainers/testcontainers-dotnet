namespace DotNet.Testcontainers.Images
{
  using System.Collections.Generic;

  public interface IFileSystem
  {
    IEnumerable<string> ReadLines(string path);

    bool FileExists(string directory, string file);

    string GetDirectoryFullName(string directory);
  }
}
