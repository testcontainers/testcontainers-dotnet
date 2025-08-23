namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an immutable collection that defines a custom strategy for
  /// composing its elements with those of another collection. This class is
  /// intended to be inherited by implementations that specify how two collections
  /// should be combined.
  /// </summary>
  /// <typeparam name="T">The type of the elements in the collection.</typeparam>
  [PublicAPI]
  public abstract class ComposableEnumerable<T> : IEnumerable<T>
  {
    private readonly IEnumerable<T> _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposableEnumerable{T}" /> class.
    /// </summary>
    /// <param name="collection">The collection of items. If <c>null</c>, an empty collection is used.</param>
    protected ComposableEnumerable(IEnumerable<T> collection)
    {
      _collection = collection ?? Array.Empty<T>();
    }

    /// <summary>
    /// Combines the current collection with the specified collection according to
    /// the composition strategy defined by the class.
    /// </summary>
    /// <remarks>
    /// The <paramref name="other" /> parameter corresponds to the previous builder
    /// configuration.
    /// </remarks>
    /// <param name="other">The incoming collection to compose with this collection.</param>
    /// <returns>A new <see cref="IEnumerable{T}" /> that contains the result of the composition.</returns>
    public abstract ComposableEnumerable<T> Compose([NotNull] IEnumerable<T> other);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the current collection.</returns>
    public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the current collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
