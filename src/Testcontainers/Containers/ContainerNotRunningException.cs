namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Linq;
  using System.Text;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when a container is not running anymore,
  /// and exited unexpectedly.
  /// </summary>
  [PublicAPI]
  public sealed class ContainerNotRunningException : Exception
  {
    private static readonly string[] LineEndings = new[] { "\r\n", "\n" };

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerNotRunningException" /> class.
    /// </summary>
    /// <param name="id">The container id.</param>
    /// <param name="stdout">The stdout.</param>
    /// <param name="stderr">The stderr.</param>
    /// <param name="exitCode">The exit code.</param>
    /// <param name="exception">The inner exception.</param>
    public ContainerNotRunningException(string id, string stdout, string stderr, long exitCode, [CanBeNull] Exception exception)
      : base(CreateMessage(id, stdout, stderr, exitCode), exception)
    {
    }

    private static string CreateMessage(string id, string stdout, string stderr, long exitCode)
    {
      var exceptionInfo = new StringBuilder(256);
      exceptionInfo.Append($"Container {id} exited with code {exitCode}.");

      if (!string.IsNullOrEmpty(stdout))
      {
        var stdoutLines = stdout
          .Split(LineEndings, StringSplitOptions.RemoveEmptyEntries)
          .Select(line => "    " + line);

        exceptionInfo.AppendLine();
        exceptionInfo.AppendLine("  Stdout: ");
        exceptionInfo.Append(string.Join(Environment.NewLine, stdoutLines));
      }

      if (!string.IsNullOrEmpty(stderr))
      {
        var stderrLines = stderr
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
