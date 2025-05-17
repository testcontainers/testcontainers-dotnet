namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Linq;
  using System.Text;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when executing a command inside a
  /// running container fails.
  /// </summary>
  [PublicAPI]
  public sealed class ExecFailedException : Exception
  {
    private static readonly string[] LineEndings = new[] { "\r\n", "\n" };

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecFailedException" /> class.
    /// </summary>
    /// <param name="execResult">The result of the failed command execution.</param>
    public ExecFailedException(ExecResult execResult)
      : base(CreateMessage(execResult))
    {
      ExecResult = execResult;
    }

    /// <summary>
    /// Gets the result of the failed command execution inside the container.
    /// </summary>
    public ExecResult ExecResult { get; }

    private static string CreateMessage(ExecResult execResult)
    {
      var exceptionInfo = new StringBuilder(256);
      exceptionInfo.Append($"Process exited with code {execResult.ExitCode}.");

      if (!string.IsNullOrEmpty(execResult.Stdout))
      {
        var stdoutLines = execResult.Stdout
          .Split(LineEndings, StringSplitOptions.RemoveEmptyEntries)
          .Select(line => "    " + line);

        exceptionInfo.AppendLine();
        exceptionInfo.AppendLine("  Stdout: ");
        exceptionInfo.Append(string.Join(Environment.NewLine, stdoutLines));
      }

      if (!string.IsNullOrEmpty(execResult.Stderr))
      {
        var stderrLines = execResult.Stderr
          .Split(LineEndings, StringSplitOptions.RemoveEmptyEntries)
          .Select(line => "    " + line);

        exceptionInfo.AppendLine();
        exceptionInfo.AppendLine("  Stderr: ");
        exceptionInfo.Append(string.Join(Environment.NewLine, stderrLines));
      }

      return exceptionInfo.ToString();
    }
  }
}
