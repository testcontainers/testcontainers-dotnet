namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents a composable collection that combines its elements by appending
  /// the elements of another collection.
  /// </summary>
  /// <typeparam name="T">The type of elements in the collection.</typeparam>
  [PublicAPI]
  public sealed class AppendEnumerable<T> : ComposableEnumerable<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AppendEnumerable{T}" /> class.
    /// </summary>
    /// <param name="collection">The collection of items. If <c>null</c>, an empty collection is used.</param>
    public AppendEnumerable(IEnumerable<T> collection)
      : base(collection) { }

    /// <inheritdoc />
    public override ComposableEnumerable<T> Compose(IEnumerable<T> other)
    {
      return new AppendEnumerable<T>(BuildConfiguration.Combine(other, this));
    }
  }
}
