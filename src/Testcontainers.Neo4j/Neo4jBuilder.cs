namespace Testcontainers.Neo4j;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class Neo4jBuilder : ContainerBuilder<Neo4jBuilder, Neo4jContainer, Neo4jConfiguration>
{
    public const string Neo4jImage = "neo4j:5.4";

    public const ushort Neo4jHttpPort = 7474;

    public const ushort Neo4jBoltPort = 7687;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jBuilder" /> class.
    /// </summary>
    public Neo4jBuilder()
        : this(new Neo4jConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private Neo4jBuilder(Neo4jConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override Neo4jConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    protected override string AcceptLicenseAgreementEnvVar { get; } = "NEO4J_ACCEPT_LICENSE_AGREEMENT";

    /// <inheritdoc />
    protected override string AcceptLicenseAgreement { get; } = "yes";

    /// <inheritdoc />
    protected override string DeclineLicenseAgreement { get; } = "no";

    /// <summary>
    /// Sets the image to the Neo4j Enterprise Edition.
    /// </summary>
    /// <remarks>
    /// When <paramref name="acceptLicenseAgreement" /> is set to <c>true</c>, the Neo4j Enterprise Edition <see href="https://neo4j.com/docs/operations-manual/current/docker/introduction/#_neo4j_editions">license</see> is accepted.
    /// If the Community Edition is explicitly used, we do not update the image.
    /// </remarks>
    /// <param name="acceptLicenseAgreement">A boolean value indicating whether the Neo4j Enterprise Edition license agreement is accepted.</param>
    /// <returns>A configured instance of <see cref="Neo4jBuilder" />.</returns>
    public Neo4jBuilder WithEnterpriseEdition(bool acceptLicenseAgreement)
    {
        const string communitySuffix = "community";

        const string enterpriseSuffix = "enterprise";

        var operatingSystems = new[] { "bullseye", "ubi9" };

        var image = DockerResourceConfiguration.Image;

        string tag;

        // If the specified image does not contain a tag (but a digest), we cannot determine the
        // actual version and append the enterprise suffix. We expect the developer to set the
        // Enterprise Edition.
        if (image.Tag == null)
        {
            tag = null;
        }
        else if (image.MatchLatestOrNightly())
        {
            tag = enterpriseSuffix;
        }
        else if (image.MatchVersion(v => v.Contains(communitySuffix)))
        {
            tag = image.Tag;
        }
        else if (image.MatchVersion(v => v.Contains(enterpriseSuffix)))
        {
            tag = image.Tag;
        }
        else if (image.MatchVersion(v => Array.Exists(operatingSystems, v.Contains)))
        {
            MatchEvaluator evaluator = match => $"{enterpriseSuffix}-{match.Value}";
            tag = Regex.Replace(image.Tag, string.Join("|", operatingSystems), evaluator, RegexOptions.None, TimeSpan.FromSeconds(1));
        }
        else
        {
            tag = $"{image.Tag}-{enterpriseSuffix}";
        }

        var enterpriseImage = new DockerImage(image.Repository, image.Registry, tag, tag == null ? image.Digest : null);

        var licenseAgreement = acceptLicenseAgreement ? AcceptLicenseAgreement : DeclineLicenseAgreement;

        return WithImage(enterpriseImage).WithEnvironment(AcceptLicenseAgreementEnvVar, licenseAgreement);
    }

    /// <inheritdoc />
    public override Neo4jContainer Build()
    {
        Validate();
        return new Neo4jContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string message = "The image '{0}' requires you to accept a license agreement.";

        base.Validate();

        Predicate<Neo4jConfiguration> licenseAgreementNotAccepted = value => value.Image.Tag != null && value.Image.Tag.Contains("enterprise")
            && (!value.Environments.TryGetValue(AcceptLicenseAgreementEnvVar, out var licenseAgreementValue) || !AcceptLicenseAgreement.Equals(licenseAgreementValue, StringComparison.Ordinal));

        _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration.Image))
            .ThrowIf(argument => licenseAgreementNotAccepted(argument.Value), argument => throw new ArgumentException(string.Format(message, DockerResourceConfiguration.Image.FullName), argument.Name));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Init()
    {
        return base.Init()
            .WithImage(Neo4jImage)
            .WithPortBinding(Neo4jHttpPort, true)
            .WithPortBinding(Neo4jBoltPort, true)
            .WithEnvironment("NEO4J_AUTH", "none")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(Neo4jHttpPort)));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Neo4jConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Neo4jConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Merge(Neo4jConfiguration oldValue, Neo4jConfiguration newValue)
    {
        return new Neo4jBuilder(new Neo4jConfiguration(oldValue, newValue));
    }
}