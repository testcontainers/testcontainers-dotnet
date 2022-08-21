namespace DotNet.Testcontainers.Builders
{
  using System.IO;
  using System.Text.RegularExpressions;

  public readonly struct CommonDirectoryPath
  {
    /// <summary>
    /// Sets the DirectoryPath to the parent directory path of the bin directory.
    /// </summary>
    public static readonly CommonDirectoryPath BuildRoot = new CommonDirectoryPath(GetParentBinPath());

    private CommonDirectoryPath(string directoryPath)
    {
      this.DirectoryPath = directoryPath;
    }

    public string DirectoryPath { get; }

    private static string GetParentBinPath()
    {
      var rootDir = Path.GetDirectoryName(System.Reflection.Assembly.
        GetExecutingAssembly().CodeBase);
      var pattern = "(?<!fil)[A-Za-z]:\\\\+[\\S\\s]*?(?=\\\\+bin)";
      var match = Regex.Match(rootDir, pattern);
      return match.Value;
    }
  }
}
