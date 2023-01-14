namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Linq;

  internal static class MatchImage
  {
    public static IImage Match(string image)
    {
      Guard.Argument(image, nameof(image))
        .NotNull()
        .NotEmpty();

      var imageComponents = image.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

      var repository = string.Join("/", imageComponents
        .Take(imageComponents.Length - 1));

      var name = imageComponents
        .Last()
        .Split(':')
        .FirstOrDefault() ?? string.Empty;

      var tag = imageComponents
        .Last()
        .Split(':')
        .LastOrDefault() ?? string.Empty;

      return new DockerImage(repository, name, tag);
    }
  }
}
