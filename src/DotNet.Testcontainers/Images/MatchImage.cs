namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Linq;

  internal static class MatchImage
  {
    public static IDockerImage Match(string image)
    {
      Guard.Argument(image, nameof(image))
        .NotNull()
        .NotEmpty();

      var dockerImageParts = image.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

      var repository = string.Join("/", dockerImageParts
        .Take(dockerImageParts.Length - 1)
        .ToArray());

      var name = dockerImageParts
        .Last()
        .Split(':')
        .FirstOrDefault() ?? string.Empty;

      var tag = dockerImageParts
        .Last()
        .Split(':')
        .Skip(1)
        .FirstOrDefault() ?? string.Empty;

      return new DockerImage(repository, name, tag);
    }
  }
}
