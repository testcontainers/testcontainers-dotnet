namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Reflection;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  /// <summary>
  /// https://github.com/testcontainers/testcontainers-dotnet/issues/1777.
  /// The Docker endpoint discovery runs once per process. The scenario therefore runs in a
  /// child process that executes <see cref="Child" /> with a fully occupied thread pool.
  /// </summary>
  public sealed class BuilderStarvationTest
  {
    private const string ChildEnvironmentVariable = "TESTCONTAINERS_BUILDER_STARVATION_CHILD";

    private const string MarkerFileEnvironmentVariable = "TESTCONTAINERS_BUILDER_STARVATION_MARKER";

    [Fact]
    public async Task ConstructingBuilderDoesNotWaitForThreadPool()
    {
      if (IsChild)
      {
        return;
      }

      var assemblyPath = Assembly.GetExecutingAssembly().Location;
      var hostPath = Process.GetCurrentProcess().MainModule?.FileName;
      var isDotnetHost = "dotnet".Equals(Path.GetFileNameWithoutExtension(hostPath), StringComparison.OrdinalIgnoreCase);

      var startInfo = new ProcessStartInfo(hostPath)
      {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
      };

      if (isDotnetHost)
      {
        startInfo.ArgumentList.Add(assemblyPath);
      }

      startInfo.ArgumentList.Add("-method");
      startInfo.ArgumentList.Add(typeof(BuilderStarvationTest).FullName + "." + nameof(Child));
      var markerFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));
      startInfo.Environment[ChildEnvironmentVariable] = bool.TrueString;
      startInfo.Environment[MarkerFileEnvironmentVariable] = markerFilePath;

      using var process = Process.Start(startInfo);
      Assert.NotNull(process);

      var stdout = process.StandardOutput.ReadToEndAsync(TestContext.Current.CancellationToken);
      var stderr = process.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);

      var exited = true;

      try
      {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromMinutes(2));
        await process.WaitForExitAsync(timeout.Token);
      }
      catch (OperationCanceledException)
      {
        exited = false;
        process.Kill(true);
      }

      var output = await stdout + Environment.NewLine + await stderr;
      Assert.True(exited && 0.Equals(process.ExitCode), output);

      // Guards against a child run that matched no test and exited with 0.
      Assert.True(File.Exists(markerFilePath), "The child process did not run the scenario." + Environment.NewLine + output);
      File.Delete(markerFilePath);
    }

    [Fact]
    public void Child()
    {
      if (!IsChild)
      {
        return;
      }

      ThreadPool.GetMinThreads(out _, out var minIo);
      ThreadPool.SetMinThreads(Environment.ProcessorCount, minIo);
      ThreadPool.SetMaxThreads(Environment.ProcessorCount, minIo);
      ThreadPool.GetMaxThreads(out var maxWorkers, out _);

      // Not disposed: queued work items still wait on the gate after this method returns.
      var gate = new ManualResetEventSlim(false);
      using var done = new ManualResetEventSlim(false);

      try
      {
        // Queue more items than workers. Pending items prove that every worker is occupied.
        for (var i = 0; i < maxWorkers * 2; i++)
        {
          ThreadPool.QueueUserWorkItem(_ => gate.Wait());
        }

        var saturated = SpinWait.SpinUntil(() => ThreadPool.PendingWorkItemCount > 0, TimeSpan.FromSeconds(10));
        Assert.True(saturated, "The thread pool was not saturated.");

        var thread = new Thread(() =>
        {
          _ = new NetworkBuilder();
          done.Set();
        })
        {
          IsBackground = true,
        };

        thread.Start();

        var completed = done.Wait(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
        gate.Set();

        Assert.True(completed, "Constructing a builder waited for the thread pool.");
        File.WriteAllText(Environment.GetEnvironmentVariable(MarkerFileEnvironmentVariable), string.Empty);
      }
      finally
      {
        gate.Set();
      }
    }

    private static bool IsChild => bool.TrueString.Equals(Environment.GetEnvironmentVariable(ChildEnvironmentVariable));
  }
}
