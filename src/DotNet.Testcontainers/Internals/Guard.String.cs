namespace DotNet.Testcontainers.Internals
{
  using System;
  using System.Diagnostics;

  /// <summary>
  /// String preconditions.
  /// </summary>
  internal static partial class Guard
  {
    /// <summary>
    /// Ensures that an argument string value is empty.
    /// </summary>
    /// <param name="argument">String argument to validate.</param>
    /// <returns>Reference to the Guard object that validates the argument preconditions.</returns>
    /// <exception cref="ArgumentException">Thrown when argument is not empty.</exception>
    [DebuggerStepThrough]
    public static ref readonly ArgumentInfo<string> Empty(in this ArgumentInfo<string> argument)
    {
      if (argument.Value.Length > 0)
      {
        throw new ArgumentException($"{argument.Name} must be empty.", argument.Name);
      }

      return ref argument;
    }

    /// <summary>
    /// Ensure that an argument string value is not empty.
    /// </summary>
    /// <param name="argument">String argument to validate.</param>
    /// <returns>Reference to the Guard object that validates the argument preconditions.</returns>
    /// <exception cref="ArgumentException">Thrown when argument is empty.</exception>
    [DebuggerStepThrough]
    public static ref readonly ArgumentInfo<string> NotEmpty(in this ArgumentInfo<string> argument)
    {
      if (argument.Value.Length == 0)
      {
        throw new ArgumentException($"{argument.Name} can not be empty.", argument.Name);
      }

      return ref argument;
    }
  }
}
