namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="ContainerConfiguration" />
  [PublicAPI]
  public sealed class ComposeConfiguration : ContainerConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeConfiguration" /> class.
    /// </summary>
    /// <param name="composeFiles">A list of Docker Compose files.</param>
    /// <param name="projectName">The Docker Compose project name.</param>
    /// <param name="projectNamePrefix">The Docker Compose project name prefix.</param>
    /// <param name="services">A list of Docker Compose services to start.</param>
    /// <param name="scaledServices">A dictionary of Docker Compose services and the number of containers they run.</param>
    /// <param name="exposedServices">A list of exposed Docker Compose service ports.</param>
    /// <param name="serviceReadiness">A list that indicates the readiness of the Docker Compose services.</param>
    /// <param name="fileCopyInclusions">A list of files and directories to copy into the container.</param>
    /// <param name="pull">A value indicating whether the Docker Compose images are pulled before the services start or not.</param>
    public ComposeConfiguration(
      IEnumerable<string> composeFiles = null,
      string projectName = null,
      string projectNamePrefix = null,
      IEnumerable<string> services = null,
      IReadOnlyDictionary<string, ushort> scaledServices = null,
      IEnumerable<ComposeExposedService> exposedServices = null,
      IEnumerable<ComposeServiceReadiness> serviceReadiness = null,
      IEnumerable<string> fileCopyInclusions = null,
      bool? pull = null)
    {
      ComposeFiles = composeFiles;
      ProjectName = projectName;
      ProjectNamePrefix = projectNamePrefix;
      Services = services;
      ScaledServices = scaledServices;
      ExposedServices = exposedServices;
      ServiceReadiness = serviceReadiness;
      FileCopyInclusions = fileCopyInclusions;
      Pull = pull;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ComposeConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
      : base(resourceConfiguration)
    {
      // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ComposeConfiguration(IContainerConfiguration resourceConfiguration)
      : base(resourceConfiguration)
    {
      // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ComposeConfiguration(ComposeConfiguration resourceConfiguration)
      : this(new ComposeConfiguration(), resourceConfiguration)
    {
      // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ComposeConfiguration(ComposeConfiguration oldValue, ComposeConfiguration newValue)
      : base(oldValue, newValue)
    {
      ComposeFiles = BuildConfiguration.Combine(oldValue.ComposeFiles, newValue.ComposeFiles);
      ProjectName = BuildConfiguration.Combine(oldValue.ProjectName, newValue.ProjectName);
      ProjectNamePrefix = BuildConfiguration.Combine(oldValue.ProjectNamePrefix, newValue.ProjectNamePrefix);
      Services = BuildConfiguration.Combine(oldValue.Services, newValue.Services);
      ScaledServices = BuildConfiguration.Combine(oldValue.ScaledServices, newValue.ScaledServices);
      ExposedServices = BuildConfiguration.Combine(oldValue.ExposedServices, newValue.ExposedServices);
      ServiceReadiness = BuildConfiguration.Combine(oldValue.ServiceReadiness, newValue.ServiceReadiness);
      FileCopyInclusions = BuildConfiguration.Combine(oldValue.FileCopyInclusions, newValue.FileCopyInclusions);
      Pull = BuildConfiguration.Combine(oldValue.Pull, newValue.Pull);
    }

    /// <summary>
    /// Gets a list of Docker Compose files.
    /// </summary>
    public IEnumerable<string> ComposeFiles { get; }

    /// <summary>
    /// Gets the Docker Compose project name.
    /// </summary>
    public string ProjectName { get; }

    /// <summary>
    /// Gets the Docker Compose project name prefix.
    /// </summary>
    public string ProjectNamePrefix { get; }

    /// <summary>
    /// Gets a list of Docker Compose services to start.
    /// </summary>
    public IEnumerable<string> Services { get; }

    /// <summary>
    /// Gets a dictionary of Docker Compose services and the number of containers they run.
    /// </summary>
    public IReadOnlyDictionary<string, ushort> ScaledServices { get; }

    /// <summary>
    /// Gets a list of exposed Docker Compose service ports.
    /// </summary>
    public IEnumerable<ComposeExposedService> ExposedServices { get; }

    /// <summary>
    /// Gets a list that indicates the readiness of the Docker Compose services.
    /// </summary>
    public IEnumerable<ComposeServiceReadiness> ServiceReadiness { get; }

    /// <summary>
    /// Gets a list of files and directories to copy into the container, relative to
    /// the directory of the first Docker Compose file.
    /// </summary>
    public IEnumerable<string> FileCopyInclusions { get; }

    /// <summary>
    /// Gets a value indicating whether the Docker Compose images are pulled before
    /// the services start or not.
    /// </summary>
    public bool? Pull { get; }
  }
}
