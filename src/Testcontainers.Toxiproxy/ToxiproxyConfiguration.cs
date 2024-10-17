namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ToxiproxyConfiguration : ContainerConfiguration
{
    public ToxiproxyConfiguration()
    {
    }

    public ToxiproxyConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    public ToxiproxyConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    public ToxiproxyConfiguration(ToxiproxyConfiguration oldValue, ToxiproxyConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}