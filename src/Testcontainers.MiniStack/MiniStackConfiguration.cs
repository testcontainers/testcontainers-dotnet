namespace Testcontainers.MiniStack;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MiniStackConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackConfiguration" /> class.
    /// </summary>
    public MiniStackConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MiniStackConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The container configuration.</param>
    public MiniStackConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old MiniStack configuration.</param>
    /// <param name="newValue">The new MiniStack configuration.</param>
    public MiniStackConfiguration(MiniStackConfiguration oldValue, MiniStackConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}