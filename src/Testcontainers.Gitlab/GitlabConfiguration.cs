namespace Testcontainers.Gitlab;

    /// <inheritdoc cref="ContainerConfiguration" />
    [PublicAPI]
    public sealed class GitlabConfiguration : ContainerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
        /// </summary>
        public GitlabConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
        /// </summary>
        /// <param name="resourceConfiguration">The Docker resource configuration.</param>
        public GitlabConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
            : base(resourceConfiguration)
        {
            // Passes the configuration upwards to the base implementations to create an updated immutable copy.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
        /// </summary>
        /// <param name="resourceConfiguration">The Docker resource configuration.</param>
        public GitlabConfiguration(IContainerConfiguration resourceConfiguration)
            : base(resourceConfiguration)
        {
            // Passes the configuration upwards to the base implementations to create an updated immutable copy.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
        /// </summary>
        /// <param name="resourceConfiguration">The Docker resource configuration.</param>
        public GitlabConfiguration(GitlabConfiguration resourceConfiguration)
            : this(new GitlabConfiguration(), resourceConfiguration)
        {
            // Passes the configuration upwards to the base implementations to create an updated immutable copy.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitlabConfiguration" /> class.
        /// </summary>
        /// <param name="oldValue">The old Docker resource configuration.</param>
        /// <param name="newValue">The new Docker resource configuration.</param>
        public GitlabConfiguration(GitlabConfiguration oldValue, GitlabConfiguration newValue)
            : base(oldValue, newValue)
        {
        }
    }