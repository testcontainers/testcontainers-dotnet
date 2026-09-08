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
    /// <param name="platform">The platform.</param>
    /// <param name="secrets">A list of build secrets.</param>
    /// <param name="sshAgents">A dictionary of SSH agent sockets or private keys.</param>
    /// <param name="contextDirectory">The context directory.</param>
    /// <param name="dockerfile">The Dockerfile.</param>
    /// <param name="dockerfileDirectory">The Dockerfile directory.</param>
    /// <param name="target">The target.</param>
    /// <param name="image">The image.</param>
    /// <param name="imageBuildPolicy">The image build policy.</param>
    /// <param name="buildArguments">A list of build arguments.</param>
    /// <param name="deleteIfExists">A value indicating whether Testcontainers removes an existing image or not.</param>
    public BuildKitImageFromDockerfileConfiguration(
      IImage cliImage = null,
      string platform = null,
      IEnumerable<BuildSecret> secrets = null,
      IReadOnlyDictionary<string, IEnumerable<string>> sshAgents = null,
      string contextDirectory = null,
      string dockerfile = null,
      string dockerfileDirectory = null,
      string target = null,
      IImage image = null,
      Func<ImageInspectResponse, bool> imageBuildPolicy = null,
      IReadOnlyDictionary<string, string> buildArguments = null,
      bool? deleteIfExists = null)
      : base(
        contextDirectory,
        dockerfile,
        dockerfileDirectory,
        target,
        image,
        imageBuildPolicy,
        buildArguments,
        deleteIfExists)
    {
      CliImage = cliImage;
      Platform = platform;
      Secrets = secrets;
      SshAgents = sshAgents;
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
      Platform = BuildConfiguration.Combine(oldValue.Platform, newValue.Platform);
      Secrets = BuildConfiguration.Combine(oldValue.Secrets, newValue.Secrets);
      SshAgents = BuildConfiguration.Combine(oldValue.SshAgents, newValue.SshAgents);
    }

    /// <inheritdoc />
    [JsonIgnore]
    public IImage CliImage { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public string Platform { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public IEnumerable<BuildSecret> Secrets { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public IReadOnlyDictionary<string, IEnumerable<string>> SshAgents { get; }
  }
}
