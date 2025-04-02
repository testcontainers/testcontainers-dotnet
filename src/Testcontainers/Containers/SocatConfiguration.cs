using System.Collections.Generic;

namespace DotNet.Testcontainers.Containers
{
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <inheritdoc cref="ContainerConfiguration" />
  [PublicAPI]
  public sealed class SocatConfiguration : ContainerConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SocatConfiguration" /> class.
    /// </summary>
    /// <param name="targets">The Socat targets.</param>
    public SocatConfiguration(
      Dictionary<int, string> targets = null)
    {
      Targets = targets;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SocatConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SocatConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
      : base(resourceConfiguration)
    {
      // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SocatConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SocatConfiguration(IContainerConfiguration resourceConfiguration)
      : base(resourceConfiguration)
    {
      // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SocatConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SocatConfiguration(SocatConfiguration resourceConfiguration)
      : this(new SocatConfiguration(), resourceConfiguration)
    {
      // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SocatConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public SocatConfiguration(SocatConfiguration oldValue, SocatConfiguration newValue)
      : base(oldValue, newValue)
    {
      Targets = BuildConfiguration.Combine(oldValue.Targets, newValue.Targets);
    }

    /// <summary>
    /// Gets the Socat exposed port.
    /// </summary>
    public Dictionary<int, string> Targets { get; }
  }
}
