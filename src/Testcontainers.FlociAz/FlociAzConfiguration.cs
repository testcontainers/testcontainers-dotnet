namespace Testcontainers.FlociAz;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class FlociAzConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzConfiguration" /> class.
    /// </summary>
    public FlociAzConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FlociAzConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The container configuration.</param>
    public FlociAzConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old FlociAz configuration.</param>
    /// <param name="newValue">The new FlociAz configuration.</param>
    public FlociAzConfiguration(FlociAzConfiguration oldValue, FlociAzConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}
