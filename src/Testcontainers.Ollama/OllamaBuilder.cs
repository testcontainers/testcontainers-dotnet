namespace Testcontainers.Ollama
{
    /// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
    [PublicAPI]
    public sealed class OllamaBuilder : ContainerBuilder<OllamaBuilder, OllamaContainer, OllamaConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
        /// </summary>
        public OllamaBuilder()
            : this(new OllamaConfiguration())
        {
            DockerResourceConfiguration = Init().DockerResourceConfiguration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
        /// </summary>
        /// <param name="resourceConfiguration">The Docker resource configuration.</param>
        private OllamaBuilder(OllamaConfiguration resourceConfiguration)
            : base(resourceConfiguration)
        {
            DockerResourceConfiguration = resourceConfiguration;
        }

        /// <inheritdoc />
        protected override OllamaConfiguration DockerResourceConfiguration { get; }

        /// <summary>
        /// Sets the Testcontainers.Ollama config.
        /// </summary>
        /// <param name="config">The Testcontainers.Ollama config.</param>
        /// <returns>A configured instance of <see cref="OllamaBuilder" />.</returns>
        public OllamaBuilder OllamaConfig(OllamaConfiguration config)
        {
            return Merge(DockerResourceConfiguration, config);
        }

        /// <inheritdoc />
        public override OllamaContainer Build()
        {
            Validate();
            return new OllamaContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
        }

        /// <inheritdoc />
        protected override void Validate()
        {
            Guard.Argument(DockerResourceConfiguration.ModelName, nameof(DockerResourceConfiguration.ModelName)).NotNull().NotEmpty();
            base.Validate();
        }

        /// <inheritdoc />
        protected override OllamaBuilder Init()
        {
            return base.Init()
                    .WithImage(new DockerImage(OllamaConfiguration.ImageName))
                    .WithPortBinding(OllamaConfiguration.DefaultPort, true)
                    .WithVolumeMount(OllamaConfiguration.DefaultVolumeName, OllamaConfiguration.DefaultVolumePath)
                ;
        }

        /// <inheritdoc />
        protected override OllamaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        {
            return Merge(DockerResourceConfiguration, new OllamaConfiguration(resourceConfiguration));
        }

        /// <inheritdoc />
        protected override OllamaBuilder Clone(IContainerConfiguration resourceConfiguration)
        {
            return Merge(DockerResourceConfiguration, new OllamaConfiguration(resourceConfiguration));
        }

        /// <inheritdoc />
        protected override OllamaBuilder Merge(OllamaConfiguration oldValue, OllamaConfiguration newValue)
        {
            return new OllamaBuilder(new OllamaConfiguration(oldValue, newValue));
        }
    
        /// <summary>
        /// Sets the name of the model to run.
        /// </summary>
        /// <param name="name">The name of the model to run.</param>
        /// <returns>A configured instance of <see cref="OllamaBuilder" />.</returns>
        /// <exception cref="ArgumentNullException">The name of the model to run is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">The name of the model to run is empty.</exception>
        /// <remarks>
        /// The name of the model to run is required.
        /// </remarks>
        public OllamaBuilder WithModelName(string name)
        {
            return Merge(DockerResourceConfiguration, new OllamaConfiguration(DockerResourceConfiguration) {ModelName = name});
        }
    }
}