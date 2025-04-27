namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// Creates a Regex cache for all ignore patterns.
  /// </summary>
  public class IgnoreFile
  {
    private static readonly ISearchAndReplace<string>[] PrepareRegex = { default(EscapeRegex), default(PrepareRecursiveWildcards), default(PrepareNonRecursiveWildcards), default(PrepareZeroOrOneQuantifier) };

    private readonly IEnumerable<KeyValuePair<Regex, bool>> _ignorePatterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoreFile" /> class.
    /// <see cref="Accepts" /> and <see cref="Denies" /> files.
    /// </summary>
    /// <param name="patterns">A list of strings with ignore patterns.</param>
    /// <param name="logger">The logger.</param>
    public IgnoreFile(IEnumerable<string> patterns, ILogger logger)
    {
      _ignorePatterns = patterns
        .AsParallel()

        // Keep the order.
        .AsOrdered()

        // Trim each line.
        .Select(line => line.Trim())

        // Remove empty line.
        .Where(line => !string.IsNullOrEmpty(line))

        // Remove comment.
        .Where(line => !line.StartsWith("#", StringComparison.Ordinal))

        // Exclude files and directories.
        .Select(line => line.TrimEnd('/'))

        // Exclude files and directories.
        .Select(line =>
        {
          const string filesAndDirectories = "/*";
          return line.EndsWith(filesAndDirectories, StringComparison.InvariantCulture) ? line.Substring(0, line.Length - filesAndDirectories.Length) : line;
        })

        // Exclude all files and directories (https://github.com/testcontainers/testcontainers-dotnet/issues/618).
        .Select(line => "*".Equals(line, StringComparison.OrdinalIgnoreCase) ? "**" : line)

        // Check if the pattern contains an optional prefix ("!"), which negates the pattern.
        .Aggregate(new List<KeyValuePair<string, bool>>(), (lines, line) =>
        {
          switch (line[0])
          {
            case '!':
              lines.Add(new KeyValuePair<string, bool>(line.Substring(1), true));
              break;
            case '/':
              lines.Add(new KeyValuePair<string, bool>(line.Substring(1), false));
              break;
            default:
              lines.Add(new KeyValuePair<string, bool>(line, false));
              break;
          }

          return lines;
        })

        // Prepare exact and partial patterns.
        .Aggregate(new List<KeyValuePair<string, bool>>(), (lines, line) =>
        {
          const string globstar = "**/";

          if (line.Key.Contains(globstar))
          {
            lines.Add(line);
            lines.Add(new KeyValuePair<string, bool>(line.Key.Replace(globstar, string.Empty), line.Value));
          }
          else
          {
            lines.Add(line);
          }

          return lines;
        })

        // Prepare regular expressions to accept and deny files.
        .Select((ignorePattern, index) =>
        {
          var key = ignorePattern.Key;
          var value = ignorePattern.Value;
          key = PrepareRegex.Aggregate(key, (current, prepareRegex) => prepareRegex.Replace(current));
          key = 0.Equals(index) ? key : $"([\\\\\\/]?({key}\\b|$))";
          key = $"^{key}";
          return new KeyValuePair<string, bool>(key, value);
        })

        // Cache regular expression to increase the performance.
        .Select(ignorePattern =>
        {
          var key = ignorePattern.Key;
          var value = ignorePattern.Value;
          return new KeyValuePair<Regex, bool>(new Regex(key, RegexOptions.None, TimeSpan.FromSeconds(1)), value);
        })
        .ToArray();

      foreach (var ignorePattern in _ignorePatterns)
      {
        logger.IgnorePatternAdded(ignorePattern.Key);
      }
    }

    /// <summary>
    /// Replaces all occurrences of a defined pattern.
    /// </summary>
    /// <typeparam name="TToReplace">Type of element that is searched and replaced.</typeparam>
    private interface ISearchAndReplace<TToReplace>
    {
      /// <summary>
      /// Replaces all occurrences of a defined pattern.
      /// </summary>
      /// <param name="input">Is searched and replaced.</param>
      /// <returns>Returns the input with all replaced occurrences of a defined pattern.</returns>
      TToReplace Replace(TToReplace input);
    }

    /// <summary>
    /// Returns true if the file path does not match any ignore pattern.
    /// </summary>
    /// <param name="file">Path to check.</param>
    /// <returns>True if the file path does not match any ignore pattern, otherwise false.</returns>
    public bool Accepts(string file)
    {
      var matches = _ignorePatterns.AsParallel().Where(ignorePattern => ignorePattern.Key.IsMatch(file)).ToArray();
      return matches.Length == 0 || matches[matches.Length - 1].Value;
    }

    /// <summary>
    /// Returns true if the file path matches any ignore pattern.
    /// </summary>
    /// <param name="file">Path to check.</param>
    /// <returns>True if the file path matches any ignore pattern, otherwise false.</returns>
    public bool Denies(string file)
    {
      return !Accepts(file);
    }

    /// <summary>
    /// Escapes a set of of metacharacters (-, [, ], /, {, }, (, ), +, ?, ., \, ^, $, |) with their \ codes.
    /// </summary>
    private readonly struct EscapeRegex : ISearchAndReplace<string>
    {
      private static readonly Regex Pattern = new Regex("[\\-\\[\\]\\/\\{\\}\\(\\)\\+\\?\\.\\\\\\^\\$\\|]", RegexOptions.None, TimeSpan.FromSeconds(1));

      /// <inheritdoc />
      public string Replace(string input)
      {
        return Pattern.Replace(input, "\\$&");
      }
    }

    /// <summary>
    /// Searches and replaces a string with recursive wildcards **.
    /// </summary>
    private readonly struct PrepareRecursiveWildcards : ISearchAndReplace<string>
    {
      /// <inheritdoc />
      public string Replace(string input)
      {
        return input.Replace("**", "(.+)");
      }
    }

    /// <summary>
    /// Searches and replaces a string with non recursive wildcards *.
    /// </summary>
    private readonly struct PrepareNonRecursiveWildcards : ISearchAndReplace<string>
    {
      private const string MatchAllExceptPathSeparator = "([^\\\\\\/]+)";

      /// <inheritdoc />
      public string Replace(string input)
      {
        // Find last non recursive wildcard in pattern.
        var index = input.LastIndexOf("*", StringComparison.Ordinal);

        // If last character is a non recursive wildcard, add the end of string regular expression.
        if (input.EndsWith("*", StringComparison.Ordinal) && index >= 0)
        {
          input = input.Remove(index, 1).Insert(index, $"{MatchAllExceptPathSeparator}?$");
          index = -1;
        }

        // Replace the last non recursive wildcard with a match-zero-or-one quantifier regular expression.
#if NETSTANDARD2_0
        if (input.Contains("*") && index >= 0)
#else
        if (input.Contains('*') && index >= 0)
#endif
        {
          input = input.Remove(index, 1).Insert(index, $"{MatchAllExceptPathSeparator}?");
        }

        // Replace remaining non recursive wildcards.
        return input.Replace("*", MatchAllExceptPathSeparator);
      }
    }

    /// <summary>
    /// Searches and replaces a string with zero-or-one quantifier ?.
    /// </summary>
    private readonly struct PrepareZeroOrOneQuantifier : ISearchAndReplace<string>
    {
      /// <inheritdoc />
      public string Replace(string input)
      {
        return input.Contains("\\?") ? $"{input.Replace("\\?", ".?")}$" : input;
      }
    }
  }
}
