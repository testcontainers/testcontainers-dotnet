namespace Testcontainers.ModuleName;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ModuleNameContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public ModuleNameContainer(ModuleNameConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}