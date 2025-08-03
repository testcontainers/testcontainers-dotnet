namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using DotNet.Testcontainers.Builders;

  /// <summary>
  /// Represents a composable dictionary that combines its elements by appending
  /// the elements of another dictionary with overwriting existing keys.
  /// </summary>
  /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
  /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
  [PublicAPI]
  public sealed class AppendOnlyDictionary<TKey, TValue> : ComposableDictionary<TKey, TValue>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AppendOnlyDictionary{TKey, TValue}" /> class.
    /// </summary>
    /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
    public AppendOnlyDictionary(IReadOnlyDictionary<TKey, TValue> dictionary)
      : base(dictionary)
    {
    }

    /// <inheritdoc />
    public override ComposableDictionary<TKey, TValue> Compose(IReadOnlyDictionary<TKey, TValue> other)
    {
      return new AppendOnlyDictionary<TKey, TValue>(BuildConfiguration.Combine(other, this));
    }
  }
}
