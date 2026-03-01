namespace DotNet.Testcontainers.Builders
{
  using JetBrains.Annotations;

  /// <summary>
  /// Extensions to convert string values to typed paths.
  /// </summary>
  [PublicAPI]
  public static class PathExtensions
  {
    /// <summary>
    /// Converts a string to a typed file path.
    /// </summary>
    /// <param name="path">The file path value.</param>
    /// <returns>The typed file path.</returns>
    [PublicAPI]
    public static FilePath AsFile(this string path)
    {
      return FilePath.Of(path);
    }

    /// <summary>
    /// Converts a string to a typed directory path.
    /// </summary>
    /// <param name="path">The directory path value.</param>
    /// <returns>The typed directory path.</returns>
    [PublicAPI]
    public static DirectoryPath AsDirectory(this string path)
    {
      return DirectoryPath.Of(path);
    }
  }
}