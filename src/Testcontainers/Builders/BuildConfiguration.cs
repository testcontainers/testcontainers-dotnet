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

      if (newValue == null || oldValue == null)
      {
        return newValue ?? oldValue;
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

      if (newValue == null || oldValue == null)
      {
        return newValue ?? oldValue;
      }

      return oldValue.Concat(newValue).ToArray();
    }

    /// <summary>
    /// Combines all existing and new configuration changes. If there are no changes,
    /// the previous configuration is returned.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="ComposableEnumerable{T}.Compose" /> on <paramref name="newValue" />
    /// to combine configurations. The existing <paramref name="oldValue" /> is passed as
    /// an argument to that method.
    /// </remarks>
    /// <param name="oldValue">The old configuration.</param>
    /// <param name="newValue">The new configuration.</param>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static ComposableEnumerable<T> Combine<T>(
      ComposableEnumerable<T> oldValue,
      ComposableEnumerable<T> newValue)
    {
      if (newValue == null && oldValue == null)
      {
        return new AppendOnlyEnumerable<T>(Array.Empty<T>());
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
        return new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());
      }

      if (newValue == null || oldValue == null)
      {
        return newValue ?? oldValue;
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
  }
}
