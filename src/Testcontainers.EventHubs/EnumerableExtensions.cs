#if NETFRAMEWORK
namespace Testcontainers.EventHubs;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        foreach (var item in source)
        {
            yield return item;
        }

        yield return element;
    }
}
#endif