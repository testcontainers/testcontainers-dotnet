namespace DotNet.Testcontainers
{
  using System;
  using System.Globalization;
  using System.Linq;

  /// <summary>
  /// A guard collection of string preconditions.
  /// </summary>
  public static partial class Guard
  {
    /// <summary>
    /// Ensures the argument value is empty.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="exceptionMessage">The exception message.</param>
    /// <returns>An instance of the <see cref="ArgumentInfo{TType}" /> struct.</returns>
    /// <exception cref="ArgumentException">Thrown when the condition is not met.</exception>
    public static ref readonly ArgumentInfo<string> Empty(in this ArgumentInfo<string> argument, string exceptionMessage = null)
    {
      if (argument.Value.Length == 0)
      {
        return ref argument;
      }

      const string message = "'{0}' must be empty.";
      throw new ArgumentException(exceptionMessage ?? string.Format(CultureInfo.InvariantCulture, message, argument.Name), argument.Name);
    }

    /// <summary>
    /// Ensures the argument value is not empty.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="exceptionMessage">The exception message.</param>
    /// <returns>An instance of the <see cref="ArgumentInfo{TType}" /> struct.</returns>
    /// <exception cref="ArgumentException">Thrown when the condition is not met.</exception>
    public static ref readonly ArgumentInfo<string> NotEmpty(in this ArgumentInfo<string> argument, string exceptionMessage = null)
    {
      if (argument.Value.Length > 0)
      {
        return ref argument;
      }

      const string message = "'{0}' cannot be empty.";
      throw new ArgumentException(exceptionMessage ?? string.Format(CultureInfo.InvariantCulture, message, argument.Name), argument.Name);
    }

    /// <summary>
    /// Ensures the argument value does not contain uppercase characters.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="exceptionMessage">The exception message.</param>
    /// <returns>An instance of the <see cref="ArgumentInfo{TType}" /> struct.</returns>
    /// <exception cref="ArgumentException">Thrown when the condition is not met.</exception>
    public static ref readonly ArgumentInfo<string> NotUppercase(in this ArgumentInfo<string> argument, string exceptionMessage = null)
    {
      if (!argument.Value.Any(char.IsUpper))
      {
        return ref argument;
      }

      const string message = "'{0}' cannot contain uppercase characters.";
      throw new ArgumentException(exceptionMessage ?? string.Format(CultureInfo.InvariantCulture, message, argument.Name), argument.Name);
    }
  }
}
