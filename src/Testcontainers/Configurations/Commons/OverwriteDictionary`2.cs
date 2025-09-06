namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a composable dictionary that combines its elements by replacing
  /// the current dictionary with the elements of another dictionary.
  /// </summary>
  /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
  /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
  [PublicAPI]
  public sealed class OverwriteDictionary<TKey, TValue> : ComposableDictionary<TKey, TValue>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OverwriteDictionary{TKey, TValue}" /> class.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    public OverwriteDictionary(IReadOnlyDictionary<TKey, TValue> dictionary)
      : base(dictionary) { }

    /// <inheritdoc />
    public override ComposableDictionary<TKey, TValue> Compose(
      IReadOnlyDictionary<TKey, TValue> other
    )
    {
      // Ignores all previous configurations.
      return this;
    }
  }
}
