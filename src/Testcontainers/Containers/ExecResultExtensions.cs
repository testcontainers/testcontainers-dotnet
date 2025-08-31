namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Threading.Tasks;

  /// <summary>
  /// Extension methods for working with <see cref="ExecResult" /> instances.
  /// </summary>
  public static class ExecResultExtensions
  {
    /// <summary>
    /// Awaits the <see cref="Task{ExecResult}" /> and throws an exception if the result's exit code is not successful.
    /// </summary>
    /// <param name="execTask">The task returning an <see cref="ExecResult" />.</param>
    /// <param name="successExitCodes">A list of exit codes that should be treated as successful. If none are provided, only exit code <c>0</c> is treated as successful.</param>
    /// <returns>The <see cref="ExecResult" /> if the exit code is in the list of success exit codes.</returns>
    /// <exception cref="ExecFailedException">Thrown if the exit code is not in the list of success exit codes.</exception>
    public static async Task<ExecResult> ThrowOnFailure(
      this Task<ExecResult> execTask,
      params long[] successExitCodes
    )
    {
      successExitCodes =
        successExitCodes == null || successExitCodes.Length == 0
          ? new long[] { 0 }
          : successExitCodes;

      var execResult = await execTask.ConfigureAwait(false);

      if (Array.IndexOf(successExitCodes, execResult.ExitCode) < 0)
      {
        throw new ExecFailedException(execResult);
      }

      return execResult;
    }
  }
}
