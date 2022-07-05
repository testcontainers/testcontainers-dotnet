namespace DotNet.Testcontainers.Builders
{
  using System.Collections.Generic;
  using System.Linq;

  internal static class BuildConfiguration
  {
    /// <summary>
    /// Returns the changed configuration object. If there is no change, the previous configuration object is returned.
    /// </summary>
    /// <param name="next">Changed configuration object.</param>
    /// <param name="previous">Previous configuration object.</param>
    /// <typeparam name="T">Any class.</typeparam>
    /// <returns>Changed configuration object. If there is no change, the previous configuration object.</returns>
    public static T Combine<T>(T next, T previous)
    {
      return next == null ? previous : next;
    }

    /// <summary>
    /// Combines all existing and new configuration changes. If there are no changes, the previous configurations are returned.
    /// </summary>
    /// <param name="next">Changed configuration.</param>
    /// <param name="previous">Previous configuration.</param>
    /// <typeparam name="T">Type of <see cref="IEnumerable{T}" />.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static IEnumerable<T> Combine<T>(IEnumerable<T> next, IEnumerable<T> previous)
      where T : class
    {
      if (next == null || previous == null)
      {
        return next ?? previous;
      }

      return previous.Concat(next).ToArray();
    }

    /// <summary>
    /// Combines all existing and new configuration changes while preserving the order of insertion.
    /// If there are no changes, the previous configurations are returned.
    /// </summary>
    /// <param name="next">Changed configuration.</param>
    /// <param name="previous">Previous configuration.</param>
    /// <typeparam name="T">Type of <see cref="IReadOnlyList{T}" />.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static IReadOnlyList<T> Combine<T>(IReadOnlyList<T> next, IReadOnlyList<T> previous)
      where T : class
    {
      if (next == null || previous == null)
      {
        return next ?? previous;
      }

      return previous.Concat(next).ToArray();
    }

    /// <summary>
    /// Combines all existing and new configuration changes. If there are no changes, the previous configurations are returned.
    /// </summary>
    /// <param name="next">Changed configuration.</param>
    /// <param name="previous">Previous configuration.</param>
    /// <typeparam name="T">Type of <see cref="IReadOnlyDictionary{TKey,TValue}" />.</typeparam>
    /// <returns>An updated configuration.</returns>
    public static IReadOnlyDictionary<T, T> Combine<T>(IReadOnlyDictionary<T, T> next, IReadOnlyDictionary<T, T> previous)
      where T : class
    {
      if (next == null || previous == null)
      {
        return next ?? previous;
      }

      return next.Concat(previous.Where(item => !next.Keys.Contains(item.Key))).ToDictionary(item => item.Key, item => item.Value);
    }
  }
}
