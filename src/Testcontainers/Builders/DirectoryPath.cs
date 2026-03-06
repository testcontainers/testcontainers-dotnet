namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a container or host directory path.
  /// </summary>
  [PublicAPI]
  public readonly record struct DirectoryPath
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryPath" /> struct.
    /// </summary>
    /// <param name="value">The directory path value.</param>
    private DirectoryPath(string value)
    {
      Value = value;
    }

    /// <summary>
    /// Gets the normalized directory path value.
    /// </summary>
    [PublicAPI]
    public string Value { get; }

    /// <summary>
    /// Creates a new <see cref="DirectoryPath" /> from the specified path.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <returns>The normalized <see cref="DirectoryPath" />.</returns>
    [PublicAPI]
    public static DirectoryPath Of(string path)
    {
      if (path == null)
      {
        throw new ArgumentNullException(nameof(path));
      }

      if (string.IsNullOrWhiteSpace(path))
      {
        throw new ArgumentException("The directory path cannot be empty.", nameof(path));
      }

      return new DirectoryPath(Unix.Instance.NormalizePath(path));
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return Value;
    }
  }
}
