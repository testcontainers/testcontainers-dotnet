namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Globalization;
  using System.Text.RegularExpressions;

  internal static class MatchImage
  {
    public static IImage Match(string image)
    {
      _ = Guard.Argument(image, nameof(image))
        .NotNull()
        .NotEmpty();

      const string invalidReferenceFormat = "Error parsing reference: '{0}' is not a valid repository/tag.";

      var referenceMatch = ReferenceRegex.Instance.Match(image);

      _ = Guard.Argument(referenceMatch, nameof(image))
        .ThrowIf(argument => !argument.Value.Success, argument => new ArgumentException(string.Format(CultureInfo.InvariantCulture, invalidReferenceFormat, image), argument.Name));

      var remoteName = referenceMatch.Groups["remote_name"].Value;
      var tag = referenceMatch.Groups["tag"].Value;
      var digest = referenceMatch.Groups["digest"].Value;

      // https://github.com/distribution/reference/blob/8c942b0459dfdcc5b6685581dd0a5a470f615bff/normalize.go#L146-L191
      var slices = remoteName.Split(['/'], 2);

      // The following part does not implement the entire Go implementation. It does
      // not resolve or set the default domain and repository prefix. This is not
      // necessary at the moment, and it is something we can address later.
      var (registry, repository) = slices.Length == 2 && slices[0].LastIndexOfAny(['.', ':']) > -1 ? (slices[0], slices[1]) : (null, remoteName);
      return new DockerImage(repository, registry, tag, digest);
    }

    // The regular expression used here is taken from the Go implementation:
    // https://github.com/distribution/reference/blob/8c942b0459dfdcc5b6685581dd0a5a470f615bff/reference.go.
    private sealed class ReferenceRegex : Regex
    {
      private const string Pattern = "^(?<remote_name>(?:(?:(?:[a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9])(?:\\.(?:[a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9]))*|\\[(?:[a-fA-F0-9:]+)\\])(?::[0-9]+)?\\/)?[a-z0-9]+(?:(?:[._]|__|[-]+)[a-z0-9]+)*(?:\\/[a-z0-9]+(?:(?:[._]|__|[-]+)[a-z0-9]+)*)*)(?::(?<tag>[\\w][\\w.-]{0,127}))?(?:@(?<digest>[A-Za-z][A-Za-z0-9]*(?:[-_+.][A-Za-z][A-Za-z0-9]*)*[:][0-9A-Fa-f]{32,}))?$";

      static ReferenceRegex()
      {
      }

      private ReferenceRegex()
        : base(Pattern, RegexOptions.Compiled, TimeSpan.FromSeconds(1))
      {
      }

      public static Regex Instance { get; }
        = new ReferenceRegex();
    }
  }
}
