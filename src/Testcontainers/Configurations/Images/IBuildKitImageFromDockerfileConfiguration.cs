namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// An image configuration that builds the image with BuildKit.
  /// </summary>
  [PublicAPI]
  public interface IBuildKitImageFromDockerfileConfiguration : IImageFromDockerfileConfiguration
  {
    /// <summary>
    /// Gets the Docker CLI image that runs the image build.
    /// </summary>
    IImage CliImage { get; }

    /// <summary>
    /// Gets a dictionary of build secrets, indexed by their build secret id.
    /// </summary>
    /// <remarks>
    /// The resource mapping target is the file inside the Docker CLI container that
    /// contains the build secret value.
    /// </remarks>
    IReadOnlyDictionary<string, IResourceMapping> Secrets { get; }

    /// <summary>
    /// Gets a dictionary of SSH agent sockets or private keys on the test host,
    /// indexed by their SSH id.
    /// </summary>
    IReadOnlyDictionary<string, IEnumerable<string>> Ssh { get; }
  }
}
