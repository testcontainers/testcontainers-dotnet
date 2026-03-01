namespace DotNet.Testcontainers.Builders
{
  using System;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a container or host file path.
  /// </summary>
  [PublicAPI]
  public readonly record struct FilePath
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FilePath" /> struct.
    /// </summary>
    /// <param name="value">The file path value.</param>
    private FilePath(string value)
    {
      Value = value;
    }

    /// <summary>
    /// Gets the normalized file path value.
    /// </summary>
    [PublicAPI]
    public string Value { get; }

    /// <summary>
    /// Creates a new <see cref="FilePath" /> from the specified path.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>The normalized <see cref="FilePath" />.</returns>
    [PublicAPI]
    public static FilePath Of(string path)
    {
      if (path == null)
      {
        throw new ArgumentNullException(nameof(path));
      }

      if (string.IsNullOrWhiteSpace(path))
      {
        throw new ArgumentException("The file path cannot be empty.", nameof(path));
      }

      return new FilePath(Unix.Instance.NormalizePath(path));
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return Value;
    }
  }
}