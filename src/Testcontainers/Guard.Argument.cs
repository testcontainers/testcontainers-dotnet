namespace DotNet.Testcontainers
{
  using System.Diagnostics;
  using JetBrains.Annotations;

  /// <summary>
  /// A guard to determine if one or more conditions are not met.
  /// </summary>
  [DebuggerStepThrough]
  [PublicAPI]
  public static partial class Guard
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentInfo{TType}" /> struct.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    /// <typeparam name="TType">The type.</typeparam>
    /// <returns>A new instance of the <see cref="ArgumentInfo{TType}" /> struct.</returns>
    public static ArgumentInfo<TType> Argument<TType>(TType value, string name)
    {
      return new ArgumentInfo<TType>(value, name);
    }

    /// <summary>
    /// An argument.
    /// </summary>
    /// <typeparam name="TType">The type.</typeparam>
    [DebuggerStepThrough]
    public readonly struct ArgumentInfo<TType>
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="ArgumentInfo{TType}" /> struct.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <param name="name">The name.</param>
      public ArgumentInfo(TType value, string name)
      {
        Value = value;
        Name = name;
      }

      /// <summary>
      /// Gets the value.
      /// </summary>
      public TType Value { get; }

      /// <summary>
      /// Gets the name.
      /// </summary>
      public string Name { get; }

      /// <summary>
      /// Checks whether the argument has a value or not.
      /// </summary>
      /// <returns>True if the argument has a value; otherwise, false.</returns>
      public bool HasValue()
      {
        return Value != null;
      }
    }
  }
}
