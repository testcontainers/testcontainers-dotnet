namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;

  public static class BuildConfiguration
  {
    /// <summary>
    /// Returns the changed configuration object. If there is no change, the previous configuration object is returned.
    /// </summary>
    /// <param name="oldValue">The old configuration object.</param>
    /// <param name="newValue">The new configuration object.</param>
    /// <typeparam name="T">Any class.</typeparam>
    /// <returns>Changed configuration object. If there is no change, the previous configuration object.</returns>
    public static T Combine<T>(T oldValue, T newValue)
    {
      return newValue == null ? oldValue : newValue;
    }

    /// <summary>
    /// Combines all existing and new configuration changes. If there are no changes, the previous configurations are returned.
    /// </summary>
    /// <param name="oldValue">The old configuration.</param>
    /// <param name="newValue">The new configuration.</param>
    /// <typeparam name="T">Type of <see cref="IEnumerable{T}" />.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static IEnumerable<T> Combine<T>(IEnumerable<T> oldValue, IEnumerable<T> newValue)
      where T : class
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
    /// Combines all existing and new configuration changes while preserving the order of insertion.
    /// If there are no changes, the previous configurations are returned.
    /// </summary>
    /// <param name="oldValue">The old configuration.</param>
    /// <param name="newValue">The new configuration.</param>
    /// <typeparam name="T">Type of <see cref="IReadOnlyList{T}" />.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static IReadOnlyList<T> Combine<T>(IReadOnlyList<T> oldValue, IReadOnlyList<T> newValue)
      where T : class
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
    /// Combines all existing and new configuration changes. If there are no changes, the previous configurations are returned.
    /// </summary>
    /// <param name="oldValue">The old configuration.</param>
    /// <param name="newValue">The new configuration.</param>
    /// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static IReadOnlyDictionary<TKey, TValue> Combine<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> oldValue, IReadOnlyDictionary<TKey, TValue> newValue)
      where TKey : class
      where TValue : class
    {
      if (newValue == null && oldValue == null)
      {
        return new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());
      }

      if (newValue == null || oldValue == null)
      {
        return newValue ?? oldValue;
      }

      return newValue.Concat(oldValue.Where(item => !newValue.Keys.Contains(item.Key))).ToDictionary(item => item.Key, item => item.Value);
    }
  }
}
