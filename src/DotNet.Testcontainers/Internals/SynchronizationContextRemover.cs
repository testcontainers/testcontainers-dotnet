namespace DotNet.Testcontainers.Internals
{
  using System;
  using System.Runtime.CompilerServices;
  using System.Threading;

  /// <summary>
  /// Removes SynchronizationContext and restores it before returning to the caller.
  /// </summary>
  /// <remarks>
  /// Find further information in this <see href="https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/">article</see>.
  /// </remarks>
  internal readonly struct SynchronizationContextRemover : INotifyCompletion
  {
    public bool IsCompleted
    {
      get
      {
        return SynchronizationContext.Current == null;
      }
    }

    public void OnCompleted(Action continuation)
    {
      var previousContext = SynchronizationContext.Current;

      try
      {
        SynchronizationContext.SetSynchronizationContext(null);
        continuation();
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(previousContext);
      }
    }

    public void GetResult()
    {
      // GetResult is required to be awaitable.
    }

    public SynchronizationContextRemover GetAwaiter()
    {
      return this;
    }
  }
}
