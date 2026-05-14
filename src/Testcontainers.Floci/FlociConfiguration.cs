namespace Testcontainers.Floci;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class FlociConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlociConfiguration" /> class.
    /// </summary>
    public FlociConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FlociConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The container configuration.</param>
    public FlociConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Floci configuration.</param>
    /// <param name="newValue">The new Floci configuration.</param>
    public FlociConfiguration(FlociConfiguration oldValue, FlociConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}