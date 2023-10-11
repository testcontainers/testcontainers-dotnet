namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Linq;

  internal static class MatchImage
  {
    private static readonly char[] SlashSeparator = { '/' };

    private static readonly char[] ColonSeparator = { ':' };

    public static IImage Match(string image)
    {
      _ = Guard.Argument(image, nameof(image))
        .NotNull()
        .NotEmpty();

      var imageComponents = image.Split(SlashSeparator, StringSplitOptions.RemoveEmptyEntries);

      var registry = string.Join("/", imageComponents.Take(imageComponents.Length - 1));

      imageComponents = imageComponents[imageComponents.Length - 1].Split(ColonSeparator, StringSplitOptions.RemoveEmptyEntries);

      if (2.Equals(imageComponents.Length))
      {
        return new DockerImage(registry, imageComponents[0], imageComponents[1]);
      }

      if (1.Equals(imageComponents.Length))
      {
        return new DockerImage(registry, imageComponents[0], string.Empty);
      }

      throw new ArgumentException("Cannot parse image: " + image, nameof(image));
    }
  }
}
