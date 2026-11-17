#if NETSTANDARD2_0
namespace DotNet.Testcontainers
{
  using System;
  using System.IO;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Provides extension methods for the <see cref="Stream" /> class
  /// that .NET Standard 2.0 does not support.
  /// </summary>
  internal static class StreamExtensions
  {
    /// <summary>
    /// Asynchronously reads a sequence of bytes from the stream into the buffer.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="buffer">The buffer to write the data into.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A task that completes when the bytes have been read, returning the number
    /// of bytes read into the buffer.
    /// </returns>
    public static async ValueTask<int> ReadAsync(this Stream stream, Memory<byte> buffer, CancellationToken ct = default)
    {
      if (MemoryMarshal.TryGetArray<byte>(buffer, out var segment))
      {
        return await stream.ReadAsync(segment.Array, segment.Offset, segment.Count, ct)
          .ConfigureAwait(false);
      }

      var bytes = new byte[buffer.Length];

      var numberOfBytes = await stream.ReadAsync(bytes, 0, bytes.Length, ct)
        .ConfigureAwait(false);

      bytes.AsSpan(0, numberOfBytes).CopyTo(buffer.Span);
      return numberOfBytes;
    }

    /// <summary>
    /// Asynchronously writes a sequence of bytes from the buffer to the stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="buffer">The buffer to read the data from.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the bytes have been written.</returns>
    public static async ValueTask WriteAsync(this Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken ct = default)
    {
      if (MemoryMarshal.TryGetArray(buffer, out var segment))
      {
        await stream.WriteAsync(segment.Array, segment.Offset, segment.Count, ct)
          .ConfigureAwait(false);
      }
      else
      {
        var bytes = buffer.ToArray();

        await stream.WriteAsync(bytes, 0, bytes.Length, ct)
          .ConfigureAwait(false);
      }
    }
  }
}
#endif
