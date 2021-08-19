namespace DotNet.Testcontainers.Containers
{
  using JetBrains.Annotations;

  /// <summary>
  /// A command exec result.
  /// </summary>
  public readonly struct ExecResult
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecResult" /> struct.
    /// </summary>
    /// <param name="stdout">The stdout output.</param>
    /// <param name="stderr">The stderr output.</param>
    /// <param name="exitCode">The exit code.</param>
    public ExecResult(string stdout, string stderr, long exitCode)
    {
      this.Stdout = stdout;
      this.Stderr = stderr;
      this.ExitCode = exitCode;
    }

    /// <summary>
    /// Gets the failure exec result.
    /// </summary>
    public static ExecResult Failure { get; }
      = new ExecResult(string.Empty, string.Empty, long.MinValue);

    /// <summary>
    /// Gets the stdout output.
    /// </summary>
    [PublicAPI]
    public string Stdout { get; }

    /// <summary>
    /// Gets the stderr output.
    /// </summary>
    [PublicAPI]
    public string Stderr { get; }

    /// <summary>
    /// Gets the exit code.
    /// </summary>
    [PublicAPI]
    public long ExitCode { get; }
  }
}
