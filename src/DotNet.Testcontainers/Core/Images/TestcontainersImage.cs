namespace DotNet.Testcontainers.Core.Images
{
  using System;
  using System.Linq;
  using DotNet.Testcontainers.Core.Parser;

  public class TestcontainersImage : IDockerImage
  {
    public TestcontainersImage()
    {
    }

    public TestcontainersImage(string image)
    {
      this.Image = image ?? throw new ArgumentNullException(nameof(image));
    }

    public TestcontainersImage(string repository, string name, string tag)
    {
      this.Repository = repository ?? throw new ArgumentNullException(nameof(repository));

      this.Name = name ?? throw new ArgumentNullException(nameof(name));

      this.Tag = tag ?? throw new ArgumentNullException(nameof(tag));

      if (string.IsNullOrEmpty(this.Tag))
      {
        this.Tag = "latest";
      }
    }

    public string Repository { get; private set; }

    public string Name { get; private set; }

    public string Tag { get; private set; }

    public string Image
    {
      get
      {
        if (!string.IsNullOrEmpty(this.Repository))
        {
          return $"{this.Repository}/{this.Name}:{this.Tag}";
        }
        else
        {
          return $"{this.Name}:{this.Tag}";
        }
      }

      set
      {
        if (string.IsNullOrWhiteSpace(value))
        {
          return;
        }

        var dockerImage = MatchImage.Matcher
          .Select(matcher => matcher.Match(value))
          .First(result => !(result is null));

        this.Repository = dockerImage.Repository;

        this.Name = dockerImage.Name;

        this.Tag = dockerImage.Tag;
      }
    }
  }
}
