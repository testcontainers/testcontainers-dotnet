namespace DotNet.Testcontainers.Internals
{
  using System;
  using System.Diagnostics;

  /// <summary>
  /// Nullability preconditions.
  /// </summary>
  internal static partial class Guard
  {
    /// <summary>
    /// Ensures that an argument value is null.
    /// </summary>
    /// <param name="argument">Argument to validate.</param>
    /// <typeparam name="TType">Type of the argument.</typeparam>
    /// <returns>Reference to the Guard object that validates the argument preconditions.</returns>
    /// <exception cref="ArgumentException">Thrown when argument is not null.</exception>
    [DebuggerStepThrough]
    public static ref readonly ArgumentInfo<TType> Null<TType>(in this ArgumentInfo<TType> argument)
      where TType : class
    {
      if (argument.HasValue())
      {
        throw new ArgumentException($"{argument.Name} must be null.", argument.Name);
      }

      return ref argument;
    }

    /// <summary>
    /// Ensures that an argument value is not null.
    /// </summary>
    /// <param name="argument">Argument to validate.</param>
    /// <typeparam name="TType">Type of the argument.</typeparam>
    /// <returns>Reference to the Guard object that validates the argument preconditions.</returns>
    /// <exception cref="ArgumentNullException">Thrown when argument is null.</exception>
    [DebuggerStepThrough]
    public static ref readonly ArgumentInfo<TType> NotNull<TType>(in this ArgumentInfo<TType> argument)
      where TType : class
    {
      if (!argument.HasValue())
      {
        throw new ArgumentNullException(argument.Name, $"{argument.Name} can not be null.");
      }

      return ref argument;
    }
  }
}
