namespace DotNet.Testcontainers.Images
{
  public interface IDockerImage
  {
    /// <summary>Gets the Docker image repository name.</summary>
    /// <value>Returns the Docker image repository name.</value>
    string Repository { get; }

    /// <summary>Gets the Docker image name.</summary>
    /// <value>Returns the Docker image name.</value>
    string Name { get; }

    /// <summary>Gets the Docker image tag.</summary>
    /// <value>Returns the Docker image tag if present or "latest" instead.</value>
    string Tag { get; }

    /// <summary>Gets or sets the full Docker image name. Splits the full Docker image name into its components and sets each properties.</summary>
    /// <value>Full Docker image name, like "foo/bar:1.0.0" or "bar:latest" based on the components values.</value>
    string FullName { get; }
  }
}
