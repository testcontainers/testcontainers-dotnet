namespace DotNet.Testcontainers.Configurations
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <inheritdoc cref="IResourceMapping" />
  internal class BinaryResourceMapping : FileResourceMapping
  {
    private readonly byte[] _resourceContent;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryResourceMapping" /> class.
    /// </summary>
    /// <param name="resourceContent">The byte array content to map in the container.</param>
    /// <param name="containerPath">The absolute path of a file to map in the container.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    public BinaryResourceMapping(byte[] resourceContent, string containerPath, UnixFileModes fileMode)
      : base(string.Empty, containerPath, fileMode)
    {
      _resourceContent = resourceContent;
    }

    /// <inheritdoc />
    public override Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
    {
      return Task.FromResult(_resourceContent);
    }
  }
}
