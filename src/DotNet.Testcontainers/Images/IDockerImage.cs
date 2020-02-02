namespace DotNet.Testcontainers.Images
{
  using System.Diagnostics.CodeAnalysis;

  public interface IDockerImage
  {
    /// <summary>Gets the Docker image repository name.</summary>
    /// <value>Returns the Docker image repository name.</value>
    [NotNull]
    string Repository { get; }

    /// <summary>Gets the Docker image name.</summary>
    /// <value>Returns the Docker image name.</value>
    [NotNull]
    string Name { get; }

    /// <summary>Gets the Docker image tag.</summary>
    /// <value>Returns the Docker image tag if present or "latest" instead.</value>
    [NotNull]
    string Tag { get; }

    /// <summary>Gets or sets the full Docker image name. Splits the full Docker image name into its components and sets each properties.</summary>
    /// <value>Full Docker image name, like "foo/bar:1.0.0" or "bar:latest" based on the components values.</value>
    [NotNull]
    string FullName { get; }
  }
}
