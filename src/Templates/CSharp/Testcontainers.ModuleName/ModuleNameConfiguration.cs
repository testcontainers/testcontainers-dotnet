namespace Testcontainers.ModuleName;

[PublicAPI]
public sealed class ModuleNameConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameConfiguration" /> class.
    /// </summary>
    public ModuleNameConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ModuleNameConfiguration(IResourceConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ModuleNameConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ModuleNameConfiguration(ModuleNameConfiguration resourceConfiguration)
        : this(new ModuleNameConfiguration(), resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ModuleNameConfiguration(ModuleNameConfiguration oldValue, ModuleNameConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}