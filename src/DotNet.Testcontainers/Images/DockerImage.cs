namespace DotNet.Testcontainers.Images
{
  using System;

  /// <inheritdoc cref="IDockerImage" />
  public sealed class DockerImage : IDockerImage
  {
    private static readonly Func<string, IDockerImage> GetDockerImage = MatchImage.Match;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public DockerImage(IDockerImage image)
      : this(image.Repository, image.Name, image.Tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>"fedora/httpd:version1.0" where "fedora" is the repository, "httpd" the name and "version1.0" the tag.</example>
    public DockerImage(string image)
      : this(GetDockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImage" /> class.
    /// </summary>
    /// <param name="repository">The Docker image repository.</param>
    /// <param name="name">The Docker image name.</param>
    /// <param name="tag">The Docker image tag.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    /// <example>"fedora/httpd:version1.0" where "fedora" is the repository, "httpd" the name and "version1.0" the tag.</example>
    public DockerImage(
      string repository,
      string name,
      string tag)
    {
      Guard.Argument(repository, nameof(repository))
        .NotNull();

      Guard.Argument(name, nameof(name))
        .NotNull()
        .NotEmpty();

      Guard.Argument(tag, nameof(tag))
        .NotNull();

      this.Repository = repository;
      this.Name = name;
      this.Tag = tag;

      if (this.Tag.Length == 0)
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
    public string FullName => this.Repository.Length == 0 ? $"{this.Name}:{this.Tag}" : $"{this.Repository}/{this.Name}:{this.Tag}";
  }
}
