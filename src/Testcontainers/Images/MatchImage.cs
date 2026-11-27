namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Globalization;

  internal static class MatchImage
  {
    public static IImage Match(string image)
    {
      _ = Guard.Argument(image, nameof(image))
        .NotNull()
        .NotEmpty();

      const string invalidReferenceFormat = "Error parsing reference: '{0}' is not a valid repository/tag.";

      var referenceMatch = RegexPatterns.ImageReference().Match(image);

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
  }
}
