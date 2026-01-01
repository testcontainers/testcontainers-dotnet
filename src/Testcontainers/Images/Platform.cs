namespace DotNet.Testcontainers.Images
{
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a container platform identifier.
  /// </summary>
  /// <remarks>
  /// The supported format for a platform value is:
  /// <c>&lt;os&gt;|&lt;arch&gt;|&lt;os&gt;/&lt;arch&gt;[/&lt;variant&gt;]</c>.
  ///
  /// You can provide either the operating system or the architecture or both.
  /// For more details, see <see href="https://github.com/containerd/platforms">containerd/platforms</see>.
  /// </remarks>
  [PublicAPI]
  public readonly struct Platform
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Platform" /> struct.
    /// </summary>
    /// <param name="value">The platform identifier.</param>
    [PublicAPI]
    public Platform(string value)
    {
      Value = value;
    }

    /// <summary>
    /// Gets the platform identifier.
    /// </summary>
    /// <remarks>
    /// A string representing the container platform in <c>containerd/platforms</c> format, or
    /// <c>null</c> if no platform was specified.
    /// </remarks>
    [PublicAPI]
    [CanBeNull]
    public string Value { get; }
  }
}
