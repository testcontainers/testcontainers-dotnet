namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Linq;

  internal static class MatchImage
  {
    private static readonly char[] AtSeparator = { '@' };

    private static readonly char[] ColonSeparator = { ':' };

    private static readonly char[] SlashSeparator = { '/' };

    public static IImage Match(string image)
    {
      _ = Guard.Argument(image, nameof(image))
        .NotNull()
        .NotEmpty();

      var imageComponents = image.Split(SlashSeparator, StringSplitOptions.RemoveEmptyEntries);

      var registry = string.Join("/", imageComponents.Take(imageComponents.Length - 1));

      imageComponents = imageComponents[imageComponents.Length - 1].Split(AtSeparator, 2, StringSplitOptions.RemoveEmptyEntries);

      var digest = imageComponents.Length == 2 ? $"@{imageComponents[1]}" : string.Empty;

      imageComponents = imageComponents[0].Split(ColonSeparator, 2, StringSplitOptions.RemoveEmptyEntries);

      if (2.Equals(imageComponents.Length))
      {
        return new DockerImage(registry, imageComponents[0], string.IsNullOrEmpty(digest) ? imageComponents[1] : $"{imageComponents[1]}{digest}");
      }

      if (1.Equals(imageComponents.Length))
      {
        return new DockerImage(registry, imageComponents[0], digest);
      }

      throw new ArgumentException("Cannot parse image: " + image, nameof(image));
    }
  }
}
