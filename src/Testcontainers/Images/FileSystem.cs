namespace DotNet.Testcontainers.Images
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;

  public class FileSystem : IFileSystem
  {
    public IEnumerable<string> ReadLines(string path)
    {
      return File.ReadLines(path);
    }

    public bool FileExists(string directory, string file)
    {
      var directoryInfo = new DirectoryInfo(directory);
      return directoryInfo.GetFiles(file, SearchOption.TopDirectoryOnly).Any();
    }

    public string GetDirectoryFullName(string directory)
    {
      return new DirectoryInfo(directory).FullName;
    }
  }
}
