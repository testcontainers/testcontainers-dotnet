namespace DotNet.Testcontainers
{
  using System;
  using System.Globalization;
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <summary>
  /// A resource instance.
  /// </summary>
  [PublicAPI]
  public abstract class Resource : IAsyncDisposable
  {
    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

    private int _disposed;

    /// <summary>
    /// Gets a value indicating whether the resource has been disposed or not.
    /// </summary>
    protected bool Disposed => 1.Equals(Interlocked.CompareExchange(ref _disposed, 1, 0));

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
      await DisposeAsyncCore()
        .ConfigureAwait(false);

      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Checks whether the resources exists or not.
    /// </summary>
    /// <returns>True if the resource exists; otherwise, false.</returns>
    protected abstract bool Exists();

    /// <summary>
    /// Creates the resource.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the resource has been created.</returns>
    protected abstract Task UnsafeCreateAsync(CancellationToken ct = default);

    /// <summary>
    /// Deletes the resource.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the resource has been deleted.</returns>
    protected abstract Task UnsafeDeleteAsync(CancellationToken ct = default);

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
    protected virtual ValueTask DisposeAsyncCore()
    {
      _semaphoreSlim.Dispose();
      return default;
    }

    /// <summary>
    /// Acquires a lock to access the resource thread-safe.
    /// </summary>
    /// <returns>An <see cref="IDisposable" /> that releases the lock on <see cref="IDisposable.Dispose" />.</returns>
    protected virtual IDisposable AcquireLock()
    {
      return new Lock(_semaphoreSlim);
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException" /> when the resources was not found.
    /// </summary>
    /// <exception cref="InvalidOperationException">The resource was not found.</exception>
    protected virtual void ThrowIfResourceNotFound()
    {
      const string message = "Could not find resource '{0}'. Please create the resource by calling StartAsync(CancellationToken) or CreateAsync(CancellationToken).";
      _ = Guard.Argument(this, GetType().Name)
        .ThrowIf(argument => !argument.Value.Exists(), argument => new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, message, argument.Name)));
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException" /> when the lock is not acquired.
    /// </summary>
    /// <exception cref="InvalidOperationException">The lock is not acquired.</exception>
    protected virtual void ThrowIfLockNotAcquired()
    {
      const string message = "Unsafe method call requires lock.";
      _ = Guard.Argument(_semaphoreSlim, nameof(_semaphoreSlim))
        .ThrowIf(argument => argument.Value.CurrentCount > 0, _ => new InvalidOperationException(message));
    }

    /// <summary>
    /// A lock to synchronize threads.
    /// </summary>
    private sealed class Lock : IDisposable
    {
      private readonly SemaphoreSlim _semaphoreSlim;

      /// <summary>
      /// Initializes a new instance of the <see cref="Lock" /> class.
      /// </summary>
      /// <param name="semaphoreSlim">The semaphore slim to synchronize threads.</param>
      public Lock(SemaphoreSlim semaphoreSlim)
      {
        _semaphoreSlim = semaphoreSlim;
        _semaphoreSlim.Wait();
      }

      public void Dispose()
      {
        _semaphoreSlim.Release();
      }
    }
  }
}
