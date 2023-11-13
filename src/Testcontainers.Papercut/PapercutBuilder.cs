namespace Testcontainers.Papercut
{
    /// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
    [PublicAPI]
    public sealed class PapercutBuilder : ContainerBuilder<PapercutBuilder, PapercutContainer, PapercutConfiguration>
    {
        private const string PapercutImage = "jijiechen/papercut:latest";
        internal const int SmtpPort = 25;
        internal const int WebInterfacePort = 37408;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Testcontainers.PapercutBuilder" /> class.
        /// </summary>
        public PapercutBuilder()
            : this(new PapercutConfiguration())
        {
            DockerResourceConfiguration = Init().DockerResourceConfiguration;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Testcontainers.PapercutBuilder" /> class.
        /// </summary>
        /// <param name="resourceConfiguration">The Docker resource configuration.</param>
        private PapercutBuilder(PapercutConfiguration resourceConfiguration)
            : base(resourceConfiguration)
        {
            DockerResourceConfiguration = resourceConfiguration;
        }

        /// <inheritdoc />
        protected override PapercutConfiguration DockerResourceConfiguration { get; }

        /// <inheritdoc />
        public override PapercutContainer Build()
        {
            Validate();
            return new PapercutContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
        }

        /// <inheritdoc />
        protected override PapercutBuilder Init()
        {
            return base.Init()
                .WithImage(PapercutImage)
                .WithPortBinding(SmtpPort, true)
                .WithPortBinding(WebInterfacePort, true)
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilHttpRequestIsSucceeded(strategy =>
                        {
                            return strategy.ForPort(WebInterfacePort).ForStatusCode(HttpStatusCode.OK);
                        })
                );
        }

        /// <inheritdoc />
        protected override PapercutBuilder Clone(
            IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        {
            return Merge(DockerResourceConfiguration, new PapercutConfiguration(resourceConfiguration));
        }

        /// <inheritdoc />
        protected override PapercutBuilder Clone(IContainerConfiguration resourceConfiguration)
        {
            return Merge(DockerResourceConfiguration, new PapercutConfiguration(resourceConfiguration));
        }

        /// <inheritdoc />
        protected override PapercutBuilder Merge(PapercutConfiguration oldValue, PapercutConfiguration newValue)
        {
            return new PapercutBuilder(new PapercutConfiguration(oldValue, newValue));
        }
    }
}