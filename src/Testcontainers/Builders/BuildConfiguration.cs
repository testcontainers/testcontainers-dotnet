namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using DotNet.Testcontainers.Configurations;

  /// <summary>
  /// Provides static utility methods for combining old and new configuration values
  /// across various collection and value types.
  /// </summary>
  public static class BuildConfiguration
  {
    private static class EmptyComposableEnumerableCache<T>
    {
      internal static readonly ComposableEnumerable<T> Instance
        = new AppendEnumerable<T>(Array.Empty<T>());
    }

    private static class EmptyReadOnlyDictionaryCache<TKey, TValue>
    {
      internal static readonly IReadOnlyDictionary<TKey, TValue> Instance
        = new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());
    }

    private static class EmptyComposableDictionaryCache<TKey, TValue>
    {
      internal static readonly ComposableDictionary<TKey, TValue> Instance
        = new AppendDictionary<TKey, TValue>(EmptyReadOnlyDictionaryCache<TKey, TValue>.Instance);
    }

    /// <summary>
    /// Returns the updated configuration value. If the new value is <c>null</c> or
    /// <c>default</c>, the old value is returned.
    /// </summary>
    /// <param name="oldValue">The old configuration value.</param>
    /// <param name="newValue">The new configuration value.</param>
    /// <typeparam name="T">Any class.</typeparam>
    /// <returns>The updated value, or the old value if unchanged.</returns>
    public static T Combine<T>(T oldValue, T newValue)
    {
      return Equals(default(T), newValue) ? oldValue : newValue;
    }

    /// <summary>
    /// Combines all existing and new configuration changes. If there are no changes,
    /// the previous configurations are returned.
    /// </summary>
    /// <param name="oldValue">The old configuration.</param>
    /// <param name="newValue">The new configuration.</param>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static IEnumerable<T> Combine<T>(
      IEnumerable<T> oldValue,
      IEnumerable<T> newValue)
    {
      if (newValue == null && oldValue == null)
      {
        return Array.Empty<T>();
      }

      if (newValue == null)
      {
        return oldValue;
      }

      if (oldValue == null)
      {
        return newValue;
      }

      if (newValue is ICollection<T> collection && collection.Count == 0)
      {
        return oldValue;
      }

      if (oldValue is not ICollection<T>)
      {
        oldValue = oldValue.ToArray();
      }

      if (newValue is not ICollection<T>)
      {
        newValue = newValue.ToArray();
      }

      return oldValue.Concat(newValue).ToArray();
    }

    /// <summary>
    /// Combines all existing and new configuration changes while preserving the
    /// order of insertion. If there are no changes, the previous configurations
    /// are returned.
    /// </summary>
    /// <param name="oldValue">The old configuration.</param>
    /// <param name="newValue">The new configuration.</param>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static IReadOnlyList<T> Combine<T>(
      IReadOnlyList<T> oldValue,
      IReadOnlyList<T> newValue)
    {
      if (newValue == null && oldValue == null)
      {
        return Array.Empty<T>();
      }

      if (newValue == null)
      {
        return oldValue;
      }

      if (oldValue == null)
      {
        return newValue;
      }

      if (newValue.Count == 0)
      {
        return oldValue;
      }

      return oldValue.Concat(newValue).ToArray();
    }

    /// <summary>
    /// Combines all existing and new configuration changes. If there are no changes,
    /// the previous configuration is returned.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="ComposableEnumerable{T}.Compose" /> on
    /// <paramref name="newValue" /> to combine configurations. The existing
    /// <paramref name="oldValue" /> is passed as an argument to that method.
    /// </remarks>
    /// <param name="oldValue">The old configuration.</param>
    /// <param name="newValue">The new configuration.</param>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static ComposableEnumerable<T> Combine<T>(
      ComposableEnumerable<T> oldValue,
      ComposableEnumerable<T> newValue)
    {
      // A container configuration created before merging follows this branch.
      // The default value must be null or an append implementation.
      // An overwrite implementation resets the collection while merging.
      if (newValue == null && oldValue == null)
      {
        return EmptyComposableEnumerableCache<T>.Instance;
      }

      if (newValue == null || oldValue == null)
      {
        return newValue ?? oldValue;
      }

      return newValue.Compose(oldValue);
    }

    /// <summary>
    /// Combines all existing and new configuration changes. If there are no changes,
    /// the previous configurations are returned.
    /// </summary>
    /// <param name="oldValue">The old configuration.</param>
    /// <param name="newValue">The new configuration.</param>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static IReadOnlyDictionary<TKey, TValue> Combine<TKey, TValue>(
      IReadOnlyDictionary<TKey, TValue> oldValue,
      IReadOnlyDictionary<TKey, TValue> newValue)
    {
      if (newValue == null && oldValue == null)
      {
        return EmptyReadOnlyDictionaryCache<TKey, TValue>.Instance;
      }

      if (newValue == null)
      {
        return oldValue;
      }

      if (oldValue == null)
      {
        return newValue;
      }

      if (newValue.Count == 0)
      {
        return oldValue;
      }

      var result = new Dictionary<TKey, TValue>(oldValue.Count + newValue.Count);

      foreach (var kvp in oldValue)
      {
        result[kvp.Key] = kvp.Value;
      }

      foreach (var kvp in newValue)
      {
        result[kvp.Key] = kvp.Value;
      }

      return new ReadOnlyDictionary<TKey, TValue>(result);
    }

    /// <summary>
    /// Combines all existing and new configuration changes. If there are no changes,
    /// the previous configuration is returned.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="ComposableDictionary{TKey, TValue}.Compose" /> on
    /// <paramref name="newValue" /> to combine configurations. The existing
    /// <paramref name="oldValue" /> is passed as an argument to that method.
    /// </remarks>
    /// <param name="oldValue">The old configuration.</param>
    /// <param name="newValue">The new configuration.</param>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static ComposableDictionary<TKey, TValue> Combine<TKey, TValue>(
      ComposableDictionary<TKey, TValue> oldValue,
      ComposableDictionary<TKey, TValue> newValue)
    {
      // A container configuration created before merging follows this branch.
      // The default value must be null or an append implementation.
      // An overwrite implementation resets the dictionary while merging.
      if (newValue == null && oldValue == null)
      {
        return EmptyComposableDictionaryCache<TKey, TValue>.Instance;
      }

      if (newValue == null || oldValue == null)
      {
        return newValue ?? oldValue;
      }

      return newValue.Compose(oldValue);
    }
  }
}
