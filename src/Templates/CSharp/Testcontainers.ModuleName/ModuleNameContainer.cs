namespace Testcontainers.ModuleName;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ModuleNameContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ModuleNameContainer(ModuleNameConfiguration configuration)
        : base(configuration)
    {
    }
}