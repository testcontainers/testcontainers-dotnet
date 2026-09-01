namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using Xunit;

  public sealed class RetryStrategyTest
  {
    private const int MaxAttempts = 3;

    [Fact]
    public async Task DoesNotRetrySuccessfulAction()
    {
      // Given
      var attempts = 0;

      var retries = 0;

      var retryStrategy = new NoDelayRetryStrategy();
      _ = retryStrategy
        .WithMaxAttempts(MaxAttempts)
        .WithRetryOn(_ => true)
        .WithOnRetry((_, _, _) => retries++);

      // When
      await retryStrategy.ExecuteAsync(_ =>
        {
          attempts++;
          return Task.CompletedTask;
        }, TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      Assert.Equal(1, attempts);
      Assert.Equal(0, retries);
      Assert.Empty(retryStrategy.Delays);
    }

    [Fact]
    public async Task RetriesFailureWhenRetryConditionMatches()
    {
      // Given
      var attempts = 0;

      var retryStrategy = new NoDelayRetryStrategy();
      _ = retryStrategy
        .WithMaxAttempts(MaxAttempts)
        .WithRetryOn(exception => exception is InvalidOperationException);

      // When
      await retryStrategy.ExecuteAsync(_ =>
        {
          attempts++;
          return attempts == 1 ? Task.FromException(new InvalidOperationException()) : Task.CompletedTask;
        }, TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task DoesNotRetryFailureWhenRetryConditionDoesNotMatch()
    {
      // Given
      var attempts = 0;

      var exception = new InvalidOperationException();

      var retryStrategy = new NoDelayRetryStrategy();
      _ = retryStrategy
        .WithMaxAttempts(MaxAttempts)
        .WithRetryOn(_ => false);

      // When
      var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() => retryStrategy.ExecuteAsync(_ =>
        {
          attempts++;
          return Task.FromException(exception);
        }, TestContext.Current.CancellationToken))
        .ConfigureAwait(true);

      // Then
      Assert.Same(exception, actualException);
      Assert.Equal(1, attempts);
      Assert.Empty(retryStrategy.Delays);
    }

    [Fact]
    public async Task SucceedsOnFinalAttempt()
    {
      // Given
      var attempts = 0;

      var retryAttempts = new List<int>();

      var retryStrategy = new NoDelayRetryStrategy();
      _ = retryStrategy
        .WithMaxAttempts(MaxAttempts)
        .WithRetryOn(_ => true)
        .WithOnRetry((attempt, _, _) => retryAttempts.Add(attempt));

      // When
      await retryStrategy.ExecuteAsync(_ =>
        {
          attempts++;
          return attempts < MaxAttempts ? Task.FromException(new InvalidOperationException()) : Task.CompletedTask;
        }, TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      Assert.Equal(MaxAttempts, attempts);
      Assert.Equal(new[] { 2, 3 }, retryAttempts);
    }

    [Fact]
    public async Task RethrowsFailureAfterMaximumAttempts()
    {
      // Given
      var attempts = 0;

      var exception = new InvalidOperationException();

      var retryStrategy = new NoDelayRetryStrategy();
      _ = retryStrategy
        .WithMaxAttempts(MaxAttempts)
        .WithRetryOn(_ => true);

      // When
      var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() => retryStrategy.ExecuteAsync(_ =>
        {
          attempts++;
          return Task.FromException(exception);
        }, TestContext.Current.CancellationToken))
        .ConfigureAwait(true);

      // Then
      Assert.Same(exception, actualException);
      Assert.Equal(MaxAttempts, attempts);
    }

    [Fact]
    public async Task UsesExponentialBackoffWithBoundedJitter()
    {
      // Given
      var retryStrategy = new NoDelayRetryStrategy();
      _ = retryStrategy
        .WithMaxAttempts(MaxAttempts)
        .WithInterval(TimeSpan.FromSeconds(1))
        .WithRetryOn(_ => true);

      // When
      _ = await Assert.ThrowsAsync<InvalidOperationException>(() => retryStrategy.ExecuteAsync(_ =>
        {
          return Task.FromException(new InvalidOperationException());
        }, TestContext.Current.CancellationToken))
        .ConfigureAwait(true);

      // Then
      Assert.Collection(retryStrategy.Delays,
        delay => Assert.InRange(delay, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(1250)),
        delay => Assert.InRange(delay, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(2500)));
    }

    [Fact]
    public async Task DoesNotStartActionWhenAlreadyCanceled()
    {
      // Given
      var attempts = 0;

      using var cts = new CancellationTokenSource();
      cts.Cancel();

      var retryStrategy = new NoDelayRetryStrategy();
      _ = retryStrategy
        .WithMaxAttempts(MaxAttempts)
        .WithRetryOn(_ => true);

      // When
      _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => retryStrategy.ExecuteAsync(_ =>
        {
          attempts++;
          return Task.CompletedTask;
        }, cts.Token))
        .ConfigureAwait(true);

      // Then
      Assert.Equal(0, attempts);
    }

    [Fact]
    public async Task DoesNotRetryCallerCancellation()
    {
      // Given
      var attempts = 0;

      using var cts = new CancellationTokenSource();

      var retryStrategy = new NoDelayRetryStrategy();
      _ = retryStrategy
        .WithMaxAttempts(MaxAttempts)
        .WithRetryOn(_ => true);

      // When
      _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => retryStrategy.ExecuteAsync(_ =>
        {
          attempts++;
          cts.Cancel();
          return Task.FromCanceled(cts.Token);
        }, cts.Token))
        .ConfigureAwait(true);

      // Then
      Assert.Equal(1, attempts);
      Assert.Empty(retryStrategy.Delays);
    }

    [Fact]
    public async Task StopsWhenCanceledDuringDelay()
    {
      // Given
      var attempts = 0;

      using var cts = new CancellationTokenSource();

      var retryStrategy = new NoDelayRetryStrategy(cts);
      _ = retryStrategy
        .WithMaxAttempts(MaxAttempts)
        .WithRetryOn(_ => true);

      // When
      _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => retryStrategy.ExecuteAsync(_ =>
        {
          attempts++;
          return Task.FromException(new InvalidOperationException());
        }, cts.Token))
        .ConfigureAwait(true);

      // Then
      Assert.Equal(1, attempts);
    }

    /// <summary>
    /// A retry strategy that records the requested delays instead of waiting for them.
    /// </summary>
    private sealed class NoDelayRetryStrategy : RetryStrategy
    {
      private readonly CancellationTokenSource _cts;

      /// <summary>
      /// Initializes a new instance of the <see cref="NoDelayRetryStrategy" /> class.
      /// </summary>
      /// <param name="cts">The cancellation token source to cancel while waiting for the next attempt.</param>
      public NoDelayRetryStrategy(CancellationTokenSource cts = null)
      {
        _cts = cts;
      }

      /// <summary>
      /// Gets the requested delays.
      /// </summary>
      public IList<TimeSpan> Delays { get; }
        = new List<TimeSpan>();

      /// <inheritdoc />
      protected override Task DelayAsync(TimeSpan delay, CancellationToken ct)
      {
        Delays.Add(delay);

        if (_cts == null)
        {
          return Task.CompletedTask;
        }

        _cts.Cancel();
        return Task.FromCanceled(ct);
      }
    }
  }
}
