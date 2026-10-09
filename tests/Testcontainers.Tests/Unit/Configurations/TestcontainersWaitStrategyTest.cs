namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public static class TestcontainersWaitStrategyTest
  {
    public sealed class Finish : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task ImmediatelyUntil()
      {
        var exception = await Record.ExceptionAsync(() => WaitStrategy.WaitUntilAsync(() => UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100), ct: TestContext.Current.CancellationToken))
          .ConfigureAwait(true);

        Assert.Null(exception);
      }

      [Fact]
      public async Task ImmediatelyWhile()
      {
        var exception = await Record.ExceptionAsync(() => WaitStrategy.WaitWhileAsync(() => WhileAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100), ct: TestContext.Current.CancellationToken))
          .ConfigureAwait(true);

        Assert.Null(exception);
      }

      public Task<bool> UntilAsync(IContainer container)
      {
        return Task.FromResult(true);
      }

      public Task<bool> WhileAsync(IContainer container)
      {
        return Task.FromResult(false);
      }
    }

    public sealed class Timeout : IWaitUntil, IWaitWhile
    {
      private int _evaluations;

      [Fact]
      public Task After100MsUntil()
      {
        return Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitUntilAsync(() => UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100), ct: TestContext.Current.CancellationToken));
      }

      [Fact]
      public Task After100MsWhile()
      {
        return Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitWhileAsync(() => WhileAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100), ct: TestContext.Current.CancellationToken));
      }

      [Fact]
      public async Task StopsEvaluationUntil()
      {
        _ = await Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitUntilAsync(() => UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100), ct: TestContext.Current.CancellationToken))
          .ConfigureAwait(true);

        await AssertEvaluationStoppedAsync()
          .ConfigureAwait(true);
      }

      [Fact]
      public async Task StopsEvaluationWhile()
      {
        _ = await Assert.ThrowsAsync<TimeoutException>(() => WaitStrategy.WaitWhileAsync(() => WhileAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100), ct: TestContext.Current.CancellationToken))
          .ConfigureAwait(true);

        await AssertEvaluationStoppedAsync()
          .ConfigureAwait(true);
      }

      public Task<bool> UntilAsync(IContainer container)
      {
        _ = Interlocked.Increment(ref _evaluations);
        return Task.FromResult(false);
      }

      public Task<bool> WhileAsync(IContainer container)
      {
        _ = Interlocked.Increment(ref _evaluations);
        return Task.FromResult(true);
      }

      private async Task AssertEvaluationStoppedAsync()
      {
        // An evaluation that is in progress when the timeout expires still completes.
        await Task.Delay(TimeSpan.FromMilliseconds(100), TestContext.Current.CancellationToken)
          .ConfigureAwait(true);

        var expected = Volatile.Read(ref _evaluations);

        await Task.Delay(TimeSpan.FromMilliseconds(250), TestContext.Current.CancellationToken)
          .ConfigureAwait(true);

        Assert.Equal(expected, Volatile.Read(ref _evaluations));
      }
    }

    public sealed class Cancel : IWaitUntil, IWaitWhile
    {
      private readonly TaskCompletionSource<bool> _evaluation = new TaskCompletionSource<bool>();

      [Fact]
      public async Task DuringEvaluationUntil()
      {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);

        var waitTask = WaitStrategy.WaitUntilAsync(() => UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMinutes(1), ct: cts.Token);

        await cts.CancelAsync()
          .ConfigureAwait(true);

        _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => waitTask)
          .ConfigureAwait(true);
      }

      [Fact]
      public async Task DuringEvaluationWhile()
      {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);

        var waitTask = WaitStrategy.WaitWhileAsync(() => WhileAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMinutes(1), ct: cts.Token);

        await cts.CancelAsync()
          .ConfigureAwait(true);

        _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => waitTask)
          .ConfigureAwait(true);
      }

      public Task<bool> UntilAsync(IContainer container)
      {
        return _evaluation.Task;
      }

      public Task<bool> WhileAsync(IContainer container)
      {
        return _evaluation.Task;
      }
    }

    public sealed class Rethrow : IWaitUntil, IWaitWhile
    {
      [Fact]
      public Task RethrowUntil()
      {
        return Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitUntilAsync(() => UntilAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100), ct: TestContext.Current.CancellationToken));
      }

      [Fact]
      public Task RethrowWhile()
      {
        return Assert.ThrowsAsync<NotImplementedException>(() => WaitStrategy.WaitWhileAsync(() => WhileAsync(null), TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(100), ct: TestContext.Current.CancellationToken));
      }

      public Task<bool> UntilAsync(IContainer container)
      {
        throw new NotImplementedException();
      }

      public Task<bool> WhileAsync(IContainer container)
      {
        throw new NotImplementedException();
      }
    }
  }
}
