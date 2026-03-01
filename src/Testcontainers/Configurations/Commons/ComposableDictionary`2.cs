namespace DotNet.Testcontainers.Configurations
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an immutable dictionary that defines a custom strategy for
  /// composing its elements with those of another dictionary. This class is
  /// intended to be inherited by implementations that specify how two dictionaries
  /// should be combined.
  /// </summary>
  /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
  /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
  [PublicAPI]
  public abstract class ComposableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
  {
    private readonly IReadOnlyDictionary<TKey, TValue> _dictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposableDictionary{TKey, TValue}" /> class.
    /// </summary>
    /// <param name="dictionary">The dictionary of items. If <c>null</c>, an empty dictionary is used.</param>
    protected ComposableDictionary(IReadOnlyDictionary<TKey, TValue> dictionary)
    {
      _dictionary = dictionary ?? new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());
    }

    /// <summary>
    /// Combines the current dictionary with the specified dictionary according to
    /// the composition strategy defined by the class.
    /// </summary>
    /// <remarks>
    /// The <paramref name="other" /> parameter corresponds to the previous builder
    /// configuration.
    /// </remarks>
    /// <param name="other">The incoming dictionary to compose with this dictionary.</param>
    /// <returns>A new <see cref="IReadOnlyDictionary{TKey, TValue}" /> that contains the result of the composition.</returns>
    public abstract ComposableDictionary<TKey, TValue> Compose([NotNull] IReadOnlyDictionary<TKey, TValue> other);

    /// <inheritdoc />
    public IEnumerable<TKey> Keys => _dictionary.Keys;

    /// <inheritdoc />
    public IEnumerable<TValue> Values => _dictionary.Values;

    /// <inheritdoc />
    public int Count => _dictionary.Count;

    /// <inheritdoc />
    public TValue this[TKey key] => _dictionary[key];

    /// <inheritdoc />
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
