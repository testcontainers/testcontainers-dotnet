namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// A connection to the container's stdout and stderr that copies the output
  /// to an <see cref="IOutputConsumer" /> until the connection is closed.
  /// </summary>
  /// <remarks>
  /// The Docker daemon writes the container's stdout and stderr to every
  /// attached client synchronously. A client that stops reading blocks the
  /// container and every other attached client, including Docker Engine API
  /// operations such as stopping or removing the container. To avoid blocking
  /// the container, this instance closes the connection as soon as it is
  /// disposed, or as soon as the copy operation ends on its own.
  /// </remarks>
  internal sealed class AttachedStream : IDisposable
  {
    private readonly MultiplexedStream _stream;

    private readonly ILogger _logger;

    private readonly string _id;

    private int _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttachedStream" /> class.
    /// </summary>
    /// <param name="stream">The attached stdout and stderr stream.</param>
    /// <param name="outputConsumer">The stdout and stderr consumer.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="id">The container id.</param>
    public AttachedStream(MultiplexedStream stream, IOutputConsumer outputConsumer, ILogger logger, string id)
    {
      _stream = stream;
      _logger = logger;
      _id = id;

      // The copy operation runs until the connection is closed. Do not pass
      // the cancellation token of the operation that attached to the container.
      // It does not cover the lifetime of the connection, and the caller might
      // dispose its cancellation token source right after the container has
      // started.
      _ = stream.CopyOutputToAsync(Stream.Null, outputConsumer.Stdout, outputConsumer.Stderr, CancellationToken.None)
        .ContinueWith(OnCopyOutputCompleted, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Detaches from the container's stdout and stderr, closing the connection.
    /// </remarks>
    public void Dispose()
    {
      if (Interlocked.Exchange(ref _disposed, 1) == 0)
      {
        _stream.Dispose();
      }
    }

    /// <summary>
    /// Closes the connection once the copy operation has ended.
    /// </summary>
    /// <param name="task">The completed copy operation.</param>
    private void OnCopyOutputCompleted(Task task)
    {
      // Read the exception to observe it. Disposing the stream ends the pending
      // read with an exception too.
      var exception = task.Exception;

      if (Interlocked.Exchange(ref _disposed, 1) != 0)
      {
        return;
      }

      // The copy operation ended on its own. Either the container's stdout and
      // stderr reached the end, or the output consumer stopped reading.
      if (exception != null)
      {
        _logger.CanNotReadDockerContainerOutput(_id, exception);
      }

      // Close the connection instead of leaving a stalled reader attached to
      // the container.
      _stream.Dispose();
    }
  }
}
