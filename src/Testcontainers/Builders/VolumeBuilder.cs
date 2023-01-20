namespace DotNet.Testcontainers.Builders
{
  using System;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker volume builder.
  /// </summary>
  /// <example>
  ///   The default configuration is equivalent to:
  ///   <code>
  ///   _ = new VolumeBuilder()
  ///     .WithDockerEndpoint(TestcontainersSettings.OS.DockerEndpointAuthConfig)
  ///     .WithLabel(DefaultLabels.Instance)
  ///     .WithCleanUp(true)
  ///     .Build();
  ///   </code>
  /// </example>
  [PublicAPI]
  public class VolumeBuilder : AbstractBuilder<VolumeBuilder, IVolume, VolumesCreateParameters, IVolumeConfiguration>, IVolumeBuilder<VolumeBuilder>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeBuilder" /> class.
    /// </summary>
    public VolumeBuilder()
      : this(new VolumeConfiguration())
    {
      this.DockerResourceConfiguration = this.Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    protected VolumeBuilder(IVolumeConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
      this.DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override IVolumeConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public VolumeBuilder WithName(string name)
    {
      return this.Merge(this.DockerResourceConfiguration, new VolumeConfiguration(name: name));
    }

    /// <inheritdoc />
    public override IVolume Build()
    {
      this.Validate();
      return new DockerVolume(this.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected sealed override VolumeBuilder Init()
    {
      return base.Init().WithName(Guid.NewGuid().ToString("D"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
      base.Validate();

      _ = Guard.Argument(this.DockerResourceConfiguration.Name, nameof(IVolumeConfiguration.Name))
        .NotNull()
        .NotEmpty();
    }

    /// <inheritdoc />
    protected override VolumeBuilder Clone(IResourceConfiguration<VolumesCreateParameters> resourceConfiguration)
    {
      return this.Merge(this.DockerResourceConfiguration, new VolumeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override VolumeBuilder Merge(IVolumeConfiguration oldValue, IVolumeConfiguration newValue)
    {
      return new VolumeBuilder(new VolumeConfiguration(oldValue, newValue));
    }
  }
}
