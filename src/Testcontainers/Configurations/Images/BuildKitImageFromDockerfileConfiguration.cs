namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using System.Text.Json.Serialization;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IBuildKitImageFromDockerfileConfiguration" />
  [PublicAPI]
  internal sealed class BuildKitImageFromDockerfileConfiguration : ImageFromDockerfileConfiguration, IBuildKitImageFromDockerfileConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="cliImage">The Docker CLI image.</param>
    /// <param name="secrets">A dictionary of build secrets.</param>
    /// <param name="ssh">A dictionary of SSH agent sockets or private keys.</param>
    /// <param name="contextDirectory">The context directory.</param>
    /// <param name="dockerfile">The Dockerfile.</param>
    /// <param name="dockerfileDirectory">The Dockerfile directory.</param>
    /// <param name="target">The target.</param>
    /// <param name="platform">The platform.</param>
    /// <param name="image">The image.</param>
    /// <param name="imageBuildPolicy">The image build policy.</param>
    /// <param name="buildArguments">A list of build arguments.</param>
    /// <param name="deleteIfExists">A value indicating whether Testcontainers removes an existing image or not.</param>
    public BuildKitImageFromDockerfileConfiguration(
      IImage cliImage = null,
      IReadOnlyDictionary<string, IResourceMapping> secrets = null,
      IReadOnlyDictionary<string, IEnumerable<string>> ssh = null,
      string contextDirectory = null,
      string dockerfile = null,
      string dockerfileDirectory = null,
      string target = null,
      string platform = null,
      IImage image = null,
      Func<ImageInspectResponse, bool> imageBuildPolicy = null,
      IReadOnlyDictionary<string, string> buildArguments = null,
      bool? deleteIfExists = null)
      : base(
        contextDirectory,
        dockerfile,
        dockerfileDirectory,
        target,
        platform,
        image,
        imageBuildPolicy,
        buildArguments,
        deleteIfExists)
    {
      CliImage = cliImage;
      Secrets = secrets;
      Ssh = ssh;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public BuildKitImageFromDockerfileConfiguration(IResourceConfiguration<ImageBuildParameters> resourceConfiguration)
      : base(resourceConfiguration)
    {
      // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public BuildKitImageFromDockerfileConfiguration(IBuildKitImageFromDockerfileConfiguration resourceConfiguration)
      : this(new BuildKitImageFromDockerfileConfiguration(), resourceConfiguration)
    {
      // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildKitImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public BuildKitImageFromDockerfileConfiguration(IBuildKitImageFromDockerfileConfiguration oldValue, IBuildKitImageFromDockerfileConfiguration newValue)
      : base(oldValue, newValue)
    {
      CliImage = BuildConfiguration.Combine(oldValue.CliImage, newValue.CliImage);
      Secrets = BuildConfiguration.Combine(oldValue.Secrets, newValue.Secrets);
      Ssh = BuildConfiguration.Combine(oldValue.Ssh, newValue.Ssh);
    }

    /// <inheritdoc />
    [JsonIgnore]
    public IImage CliImage { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public IReadOnlyDictionary<string, IResourceMapping> Secrets { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public IReadOnlyDictionary<string, IEnumerable<string>> Ssh { get; }
  }
}
