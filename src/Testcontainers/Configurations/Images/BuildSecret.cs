namespace DotNet.Testcontainers.Configurations
{
  using System.IO;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <summary>
  /// A build secret that BuildKit exposes to the Docker image build.
  /// </summary>
  /// <remarks>
  /// The secret is mounted into the build with
  /// <c>RUN --mount=type=secret,id=&lt;id&gt;</c>. BuildKit does not add it to a
  /// layer of the built image.
  /// </remarks>
  [PublicAPI]
  public sealed class BuildSecret
  {
    private readonly IResourceMapping _resourceMapping;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildSecret" /> class.
    /// </summary>
    /// <param name="id">The build secret id.</param>
    /// <param name="value">The build secret value.</param>
    public BuildSecret(string id, string value)
      : this(id, new BinaryResourceMapping(Encoding.UTF8.GetBytes(value), GetFilePath(id), 0, 0, Unix.FileMode600))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildSecret" /> class.
    /// </summary>
    /// <param name="id">The build secret id.</param>
    /// <param name="source">The file on the test host that contains the build secret value.</param>
    public BuildSecret(string id, FileInfo source)
      : this(id, new FileResourceMapping(source.FullName, GetFilePath(id), 0, 0, Unix.FileMode600))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildSecret" /> class.
    /// </summary>
    /// <param name="id">The build secret id.</param>
    /// <param name="resourceMapping">The resource mapping that provides the build secret value.</param>
    private BuildSecret(string id, IResourceMapping resourceMapping)
    {
      Id = id;
      _resourceMapping = resourceMapping;
    }

    /// <summary>
    /// Gets the build secret id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the path of the file inside the Docker CLI container that contains the
    /// build secret value.
    /// </summary>
    /// <remarks>
    /// The build secret value is copied into the Docker CLI container that runs the
    /// image build, not into the build context. It never becomes part of the build
    /// context tar archive or of a layer of the built image.
    /// </remarks>
    internal string FilePath
    {
      get
      {
        return _resourceMapping.Target;
      }
    }

    /// <summary>
    /// Gets the path of the file on the test host that contains the build secret
    /// value.
    /// </summary>
    /// <remarks>
    /// The path is empty if the build secret value is set directly instead of read
    /// from a file.
    /// </remarks>
    internal string SourceFilePath
    {
      get
      {
        return _resourceMapping.Source;
      }
    }

    /// <summary>
    /// Gets the Unix file mode of the file inside the Docker CLI container that
    /// contains the build secret value.
    /// </summary>
    internal UnixFileModes FileMode
    {
      get
      {
        return _resourceMapping.FileMode;
      }
    }

    /// <summary>
    /// Gets the build secret value.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the build secret value has been read.</returns>
    internal Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
    {
      return _resourceMapping.GetAllBytesAsync(ct);
    }

    /// <summary>
    /// Gets the path of the file inside the Docker CLI container that contains the
    /// value of the build secret.
    /// </summary>
    /// <param name="id">The build secret id.</param>
    /// <returns>The path of the file inside the Docker CLI container.</returns>
    private static string GetFilePath(string id)
    {
      return string.Join("/", string.Empty, "tmp", "testcontainers", "secrets", id);
    }
  }
}
