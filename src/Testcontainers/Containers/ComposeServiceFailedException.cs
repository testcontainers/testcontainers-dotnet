namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when one or more Docker Compose
  /// services did not start successfully.
  /// </summary>
  [PublicAPI]
  public sealed class ComposeServiceFailedException : Exception
  {
    private static readonly string[] LineEndings = { "\r\n", "\n" };

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeServiceFailedException" /> class.
    /// </summary>
    /// <param name="projectName">The Docker Compose project name.</param>
    /// <param name="failures">The exception of each Docker Compose service that failed, indexed by its display name.</param>
    public ComposeServiceFailedException(string projectName, IEnumerable<KeyValuePair<string, Exception>> failures)
      : this(projectName, failures.ToArray())
    {
    }

    private ComposeServiceFailedException(string projectName, IReadOnlyCollection<KeyValuePair<string, Exception>> failures)
      : base(CreateMessage(projectName, failures), new AggregateException(failures.Select(failure => failure.Value)))
    {
    }

    private static string CreateMessage(string projectName, IReadOnlyCollection<KeyValuePair<string, Exception>> failures)
    {
      var serviceNames = failures.Select(failure => failure.Key);

      var exceptionInfo = new StringBuilder(256);
      exceptionInfo.Append($"The Docker Compose service(s) '{string.Join("', '", serviceNames)}' in the project '{projectName}' did not start successfully.");

      foreach (var failure in failures)
      {
        var failureLines = failure.Value.Message
          .Split(LineEndings, StringSplitOptions.RemoveEmptyEntries)
          .Select(line => "    " + line);

        exceptionInfo.AppendLine();
        exceptionInfo.AppendLine($"  {failure.Key}: ");
        exceptionInfo.Append(string.Join(Environment.NewLine, failureLines));
      }

      return exceptionInfo.ToString();
    }
  }
}
