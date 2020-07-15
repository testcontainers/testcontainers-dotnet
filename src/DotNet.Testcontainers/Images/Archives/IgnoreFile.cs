namespace DotNet.Testcontainers.Images.Archives
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Services;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// Creates a Regex cache for all ignore patterns.
  /// </summary>
  internal class IgnoreFile
  {
    private static readonly ILogger<IgnoreFile> Logger = TestcontainersHostService.GetLogger<IgnoreFile>();

    private static readonly ISearchAndReplace<string>[] PrepareRegex = { new EscapeRegex(), new PrepareRecursiveWildcards(), new PrepareNonRecursiveWildcards(), new PrepareZeroOrOneQuantifier() };

    private readonly IEnumerable<KeyValuePair<Regex, bool>> ignorePatterns;

    /// <summary>
    /// Creates an instance of <see cref="IgnoreFile" /> and converts all string patterns into regular expressions.
    /// <see cref="Accepts" /> and <see cref="Denies" /> files.
    /// </summary>
    /// <param name="patterns">String array with ignore patterns.</param>
    public IgnoreFile(params string[] patterns)
    {
      this.ignorePatterns = patterns
        .AsParallel()
        // Keep the order.
        .AsOrdered()
        // Trim each line.
        .Select(line => line.Trim())
        // Remove empty line.
        .Where(line => !string.IsNullOrEmpty(line))
        // Remove comment.
        .Where(line => !line.StartsWith("#"))
        // Check if the pattern contains an optional prefix ("!"), which negates the pattern.
        .Aggregate(new List<KeyValuePair<string, bool>>(), (lines, line) =>
        {
          switch (line.First())
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
          var (key, value) = line;

          lines.AddRange(key
            .Split('/')
            .Skip(1)
            .Prepend(key)
            .Select(ignorePattern => new KeyValuePair<string, bool>(ignorePattern, value)));

          return lines;
        })
        // Prepare regular expressions to accept and deny files.
        .Select((ignorePattern, index) =>
        {
          var (key, value) = ignorePattern;
          key = PrepareRegex.Aggregate(key, (current, prepareRegex) => prepareRegex.Replace(current));
          key = 0.Equals(index) ? key : $"([\\\\\\/]?({key}\\b|$))";
          key = $"^{key}";
          return new KeyValuePair<string, bool>(key, value);
        })
        // Compile and cache regular expression to increase the performance.
        .Select(ignorePattern =>
        {
          var (key, value) = ignorePattern;
          return new KeyValuePair<Regex, bool>(new Regex(key, RegexOptions.Compiled), value);
        })
        .ToArray();

      foreach (var ignorePattern in this.ignorePatterns)
      {
        Logger.LogInformation($"Pattern {ignorePattern.Key} added to Regex cache.");
      }
    }

    /// <summary>
    /// Returns true if the file path does not match any ignore pattern.
    /// </summary>
    /// <param name="file">Path to check.</param>
    /// <returns>True if the file path does not match any ignore pattern, otherwise false.</returns>
    public bool Accepts(string file)
    {
      var matches = this.ignorePatterns.AsParallel().Where(ignorePattern => ignorePattern.Key.IsMatch(file)).ToArray();
      return !matches.Any() || matches.Last().Value;
    }

    /// <summary>
    /// Returns true if the file path matches any ignore pattern.
    /// </summary>
    /// <param name="file">Path to check.</param>
    /// <returns>True if the file path matches any ignore pattern, otherwise false.</returns>
    public bool Denies(string file)
    {
      return !this.Accepts(file);
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
    /// Escapes a set of of metacharacters (-, [, ], /, {, }, (, ), +, ?, ., \, ^, $, |) with their \ codes.
    /// </summary>
    private readonly struct EscapeRegex : ISearchAndReplace<string>
    {
      private static readonly Regex Pattern = new Regex("[\\-\\[\\]\\/\\{\\}\\(\\)\\+\\?\\.\\\\\\^\\$\\|]", RegexOptions.Compiled);

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
        if (input.EndsWith("*") && index >= 0)
        {
          input = input.Remove(index, 1).Insert(index, $"{MatchAllExceptPathSeparator}?$");
          index = -1;
        }

        // Replace the last non recursive wildcard with a match-zero-or-one quantifier regular expression.
        if (input.Contains("*") && index >= 0)
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
