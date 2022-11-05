namespace DotNet.Testcontainers.Configurations
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <inheritdoc cref="IResourceMapping" />
  internal class BinaryResourceMapping : FileResourceMapping
  {
    private readonly byte[] resourceContent;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryResourceMapping" /> class.
    /// </summary>
    /// <param name="resourceContent">The byte array content to map in the container.</param>
    /// <param name="containerPath">The absolute path of a file to map in the container.</param>
    public BinaryResourceMapping(byte[] resourceContent, string containerPath)
      : base(string.Empty, containerPath)
    {
      this.resourceContent = resourceContent;
    }

    /// <inheritdoc />
    public override Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
    {
      return Task.FromResult(this.resourceContent);
    }
  }
}
