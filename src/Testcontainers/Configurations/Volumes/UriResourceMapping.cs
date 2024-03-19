namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;

  /// <inheritdoc cref="IResourceMapping" />
  internal sealed class UriResourceMapping : IResourceMapping
  {
    private readonly Uri _uri;

    /// <summary>
    /// Initializes a new instance of the <see cref="UriResourceMapping" /> class.
    /// </summary>
    /// <param name="uri">The URL of the file to download.</param>
    /// <param name="containerPath">The absolute path of the file to map in the container.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    public UriResourceMapping(Uri uri, string containerPath, UnixFileModes fileMode)
    {
      _uri = uri;
      Type = MountType.Bind;
      Source = uri.AbsoluteUri;
      Target = containerPath;
      FileMode = fileMode;
      AccessMode = AccessMode.ReadOnly;
    }

    /// <inheritdoc />
    public MountType Type { get; }

    /// <inheritdoc />
    public AccessMode AccessMode { get; }

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string Target { get; }

    /// <inheritdoc />
    public UnixFileModes FileMode { get; }

    /// <inheritdoc />
    public Task CreateAsync(CancellationToken ct = default) => Task.CompletedTask;

    /// <inheritdoc />
    public Task DeleteAsync(CancellationToken ct = default) => Task.CompletedTask;

    /// <inheritdoc />
    public async Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
    {
      using (var httpClient = new HttpClient())
      {
#if NET6_0_OR_GREATER
        return await httpClient.GetByteArrayAsync(_uri, ct)
          .ConfigureAwait(false);
#else
        return await httpClient.GetByteArrayAsync(_uri)
          .ConfigureAwait(false);
#endif
      }
    }
  }
}
