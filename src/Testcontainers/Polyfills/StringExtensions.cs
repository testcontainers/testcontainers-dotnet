#if NETSTANDARD2_0
namespace DotNet.Testcontainers
{
  /// <summary>
  /// Provides extension methods for the <see cref="string" /> class
  /// that .NET Standard 2.0 does not support.
  /// </summary>
  internal static class StringExtensions
  {
    /// <summary>
    /// Determines whether the string starts with the specified character.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="character">The character to compare.</param>
    /// <returns>
    /// True if the string starts with the specified character; otherwise, false.
    /// </returns>
    public static bool StartsWith(this string value, char character)
    {
      return value.Length > 0 && value[0] == character;
    }
  }
}
#endif
