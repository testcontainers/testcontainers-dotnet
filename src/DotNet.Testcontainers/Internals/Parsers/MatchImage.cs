namespace DotNet.Testcontainers.Internals.Parsers
{
  using System.Linq;
  using DotNet.Testcontainers.Images;

  internal static class MatchImage
  {
    public static IDockerImage Match(string image)
    {
      Guard.Argument(image, nameof(image))
        .NotNull()
        .NotEmpty();

      var dockerImageParts = image.Split('/');
      return new DockerImage(
        string.Join('/',
          dockerImageParts
            .Take(dockerImageParts.Length - 1)
            .ToArray()),
          dockerImageParts
            .Last()
            .Split(':')
            .FirstOrDefault() ?? string.Empty,
          dockerImageParts
            .Last()
            .Split(':')
            .Skip(1)
            .FirstOrDefault() ?? string.Empty
        );
    }
  }
}
