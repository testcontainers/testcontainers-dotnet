namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Linq;
  using DotNet.Testcontainers.Internals.Parsers;

  /// <inheritdoc cref="IDockerImage" />
  public sealed class DockerImage : IDockerImage
  {
    private static readonly MatchImage[] MatchImages = { new MatchImageRegistryTag(), new MatchImageRegistryLatest(), new MatchImageRepositoryTag(), new MatchImageRepositoryLatest(), new MatchImageTag(), new MatchImage() };

    private static readonly Func<string, IDockerImage> GetDockerImage = image => MatchImages.Select(matcher => matcher.Match(image)).First(result => result != null);

    public DockerImage(IDockerImage image) : this(image.Repository, image.Name, image.Tag)
    {
    }

    public DockerImage(string image) : this(GetDockerImage(image))
    {
    }

    public DockerImage(string repository, string name, string tag)
    {
      this.Repository = repository ?? throw new ArgumentNullException(nameof(repository));

      this.Name = name ?? throw new ArgumentNullException(nameof(name));

      this.Tag = tag ?? throw new ArgumentNullException(nameof(tag));

      if (string.IsNullOrEmpty(this.Tag))
      {
        this.Tag = "latest";
      }
    }

    /// <inheritdoc />
    public string Repository { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Tag { get; }

    /// <inheritdoc />
    public string FullName => string.IsNullOrEmpty(this.Repository) ? $"{this.Name}:{this.Tag}" : $"{this.Repository}/{this.Name}:{this.Tag}";
  }
}
