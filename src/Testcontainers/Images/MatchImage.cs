namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Linq;

  internal static class MatchImage
  {
    public static IImage Match(string image)
    {
      _ = Guard.Argument(image, nameof(image))
        .NotNull()
        .NotEmpty();

      var imageComponents = image
        .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

      var repository = string.Join("/", imageComponents
        .Take(imageComponents.Length - 1));

      var name = imageComponents
        .Last()
        .Split(':')
        .DefaultIfEmpty(string.Empty)
        .First();

      var tag = imageComponents
        .Last()
        .Split(':')
        .Skip(1)
        .DefaultIfEmpty(string.Empty)
        .First();

      return new DockerImage(repository, name, tag);
    }
  }
}
