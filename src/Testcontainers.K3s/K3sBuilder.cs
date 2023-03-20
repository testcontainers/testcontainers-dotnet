namespace Testcontainers.K3s;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class K3sBuilder : ContainerBuilder<K3sBuilder, K3sContainer, K3sConfiguration>
{
    public const int KubeSecurePort = 6443;
    public const int RancherWebhookPort = 8443;
    public const string RancherImage = "rancher/k3s:v1.26.2-k3s1";
    public const string SuccessMessage = ".*Node controller sync successful.*";

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sBuilder" /> class.
    /// </summary>
    public K3sBuilder() : this(new K3sConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private K3sBuilder(K3sConfiguration resourceConfiguration) : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override K3sConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override K3sContainer Build()
    {
        Validate();
        return new K3sContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override K3sBuilder Init()
    {
        return base.Init().WithImage(RancherImage)
            .WithPortBinding(KubeSecurePort)
            .WithPortBinding(RancherWebhookPort)
            .WithPrivileged(true)
            .WithCreateParameterModifier(it => it.HostConfig.CgroupnsMode = "host")
            .WithBindMount("/sys/fs/cgroup", "/sys/fs/cgroup", AccessMode.ReadWrite)
            .WithTmpfsMount("/run")
            .WithTmpfsMount("/var/run")
            .WithCommand("server", "--disable=traefik")
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new LogMessageWaitStrategy(SuccessMessage)));
    }

    /// <inheritdoc />
    protected override K3sBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new K3sConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override K3sBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new K3sConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override K3sBuilder Merge(K3sConfiguration oldValue, K3sConfiguration newValue)
    {
        return new K3sBuilder(new K3sConfiguration(oldValue, newValue));
    }

    public sealed class LogMessageWaitStrategy : IWaitUntil {
        private readonly string _regex;

        // timeout in seconds
        private readonly int _timeOut;

        public LogMessageWaitStrategy(string regex, int timeOut = 60) {
            _regex = regex ?? throw new ArgumentException($"{nameof(regex)} is null or empty.");
            _timeOut = timeOut;
        }

        public async Task<bool> UntilAsync(IContainer container) {
            var regex = new Regex(_regex);
            var sw = new Stopwatch();

            sw.Start();

            while (true) {
                var (stdout, stderr) = await container.GetLogs().ConfigureAwait(false);
                if (regex.IsMatch(stdout) || regex.IsMatch(stderr)) {
                    return true;
                }

                if (sw.Elapsed.Seconds > _timeOut) {
                    throw new TimeoutException();
                }
            }
        }
    }
}