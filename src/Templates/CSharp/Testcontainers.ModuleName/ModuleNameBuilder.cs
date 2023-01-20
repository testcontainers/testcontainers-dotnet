namespace Testcontainers.ModuleName;

[PublicAPI]
public sealed class ModuleNameBuilder : ContainerBuilder<ModuleNameBuilder, ModuleNameContainer, ModuleNameConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameBuilder" /> class.
    /// </summary>
    public ModuleNameBuilder()
        : this(new ModuleNameConfiguration())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ModuleNameBuilder(ModuleNameConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <inheritdoc />
    public override ModuleNameContainer Build()
    {
        return new ModuleNameContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override ModuleNameBuilder Clone(IResourceConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ModuleNameConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ModuleNameBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ModuleNameConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ModuleNameBuilder Merge(ModuleNameConfiguration oldValue, ModuleNameConfiguration newValue)
    {
        return new ModuleNameBuilder(new ModuleNameConfiguration(oldValue, newValue));
    }
}