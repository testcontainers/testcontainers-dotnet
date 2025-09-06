namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a composable dictionary that combines its elements by appending
  /// the elements of another dictionary with overwriting existing keys.
  /// </summary>
  /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
  /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
  [PublicAPI]
  public sealed class AppendDictionary<TKey, TValue> : ComposableDictionary<TKey, TValue>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AppendDictionary{TKey,TValue}" /> class.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    public AppendDictionary(IReadOnlyDictionary<TKey, TValue> dictionary)
      : base(dictionary) { }

    /// <inheritdoc />
    public override ComposableDictionary<TKey, TValue> Compose(
      IReadOnlyDictionary<TKey, TValue> other
    )
    {
      return new AppendDictionary<TKey, TValue>(BuildConfiguration.Combine(other, this));
    }
  }
}
