namespace DotNet.Testcontainers
{
  using System.Diagnostics;
  using System.Runtime.CompilerServices;

  /// <summary>
  /// Validates an argument preconditions.
  /// </summary>
  internal static partial class Guard
  {
    /// <summary>
    /// Creates a Guard object that validates argument preconditions.
    /// </summary>
    /// <param name="value">Argument value.</param>
    /// <param name="name">Argument name.</param>
    /// <typeparam name="TType">Type of the argument.</typeparam>
    /// <returns>A Guard object that validates argument preconditions.</returns>
    public static ArgumentInfo<TType> Argument<TType>(TType value, string name)
    {
      return new ArgumentInfo<TType>(value, name);
    }

    /// <summary>
    /// Represents an argument.
    /// </summary>
    /// <typeparam name="TType">Type of the argument.</typeparam>
    public readonly struct ArgumentInfo<TType>
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="ArgumentInfo{TType}" /> struct.
      /// </summary>
      /// <param name="value">Argument value.</param>
      /// <param name="name">Argument name.</param>
      [DebuggerStepThrough]
      public ArgumentInfo(TType value, string name)
      {
        this.Value = value;
        this.Name = name;
      }

      /// <summary>
      /// Gets the argument value.
      /// </summary>
      public TType Value { get; }

      /// <summary>
      /// Gets the argument name.
      /// </summary>
      public string Name { get; }

      /// <summary>
      /// Checks whether the argument value is null or not.
      /// </summary>
      /// <returns>True if the argument value is not null, otherwise false.</returns>
      [DebuggerStepThrough]
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool HasValue()
      {
        return this.Value != null;
      }
    }
  }
}
