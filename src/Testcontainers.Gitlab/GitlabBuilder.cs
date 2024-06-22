namespace Testcontainers.Gitlab;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class GitlabBuilder : ContainerBuilder<GitlabBuilder, GitlabContainer, GitlabConfiguration>
{
    /// <summary>
    /// This is the default image for gitlab community edition in version 16.11.1 .
    /// </summary>
    public const string GitlabImage = "gitlab/gitlab-ce:16.11.1-ce.0";
    /// <summary>
    /// This port is used for http communication to gitlab instance.
    /// </summary>

    public const ushort GitlabHttpPort = 80;
    /// <summary>
    /// This port is used for ssh communication to gitlab instance.
    /// </summary>
    public const ushort GitlabSshPort = 22;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitlabBuilder" /> class.
    /// </summary>
    public GitlabBuilder()
        : this(new GitlabConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitlabBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private GitlabBuilder(GitlabConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override GitlabConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override GitlabContainer Build()
    {
        Validate();
        return new GitlabContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override GitlabBuilder Init()
    {
        return base.Init()
            .WithImage(GitlabImage)
            .WithPortBinding(GitlabHttpPort, true)
            .WithPortBinding(GitlabSshPort, true)
            .WithEnvironment("GITLAB_ROOT_PASSWORD", DockerResourceConfiguration.Password)
            .WithWaitStrategy(Wait.ForUnixContainer()
               .UntilPortIsAvailable(80)
               .UntilPortIsAvailable(22)
               .UntilContainerIsHealthy()
               .UntilHttpRequestIsSucceeded(request => request.ForPath("/users/sign_in").ForStatusCode(HttpStatusCode.OK)));
    }

    /// <summary>
    /// Sets the Gitlab password.
    /// </summary>
    /// <param name="password">The Gitlab password.</param>
    /// <returns>A configured instance of <see cref="GitlabBuilder" />.</returns>
    public GitlabBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new GitlabConfiguration(password: password));
    }

    /// <inheritdoc />
    protected override void Validate() => base.Validate();

    /// <inheritdoc />
    protected override GitlabBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        => Merge(DockerResourceConfiguration, new GitlabConfiguration(resourceConfiguration));

    /// <inheritdoc />
    protected override GitlabBuilder Clone(IContainerConfiguration resourceConfiguration)
        => Merge(DockerResourceConfiguration, new GitlabConfiguration(resourceConfiguration));

    /// <inheritdoc />
    protected override GitlabBuilder Merge(GitlabConfiguration oldValue, GitlabConfiguration newValue)
        => new(new GitlabConfiguration(oldValue, newValue));
}