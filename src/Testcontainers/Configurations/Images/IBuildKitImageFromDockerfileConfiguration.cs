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
    /// Gets the platform to build the image for.
    /// </summary>
    string Platform { get; }

    /// <summary>
    /// Gets a list of build secrets.
    /// </summary>
    IEnumerable<BuildSecret> Secrets { get; }

    /// <summary>
    /// Gets a dictionary of SSH agent sockets or private keys on the test host,
    /// indexed by their SSH agent id.
    /// </summary>
    IReadOnlyDictionary<string, IEnumerable<string>> SshAgents { get; }
  }
}
