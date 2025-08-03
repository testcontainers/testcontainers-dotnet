namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a composable collection that combines its elements by replacing
  /// the current collection with the elements of another collection.
  /// </summary>
  /// <typeparam name="T">The type of elements in the collection.</typeparam>
  [PublicAPI]
  public sealed class OverwriteEnumerable<T> : ComposableEnumerable<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OverwriteEnumerable{T}" /> class.
    /// </summary>
    /// <param name="collection">The collection of items. If <c>null</c>, an empty collection is used.</param>
    public OverwriteEnumerable(IEnumerable<T> collection)
      : base(collection)
    {
    }

    /// <inheritdoc />
    public override ComposableEnumerable<T> Compose(IEnumerable<T> other)
    {
      // Ignores all previous configurations.
      return this;
    }
  }
}
