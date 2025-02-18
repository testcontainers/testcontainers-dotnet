namespace TestContainers.Smtp4Dev;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class Smtp4DevContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Smtp4DevContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public Smtp4DevContainer(Smtp4DevConfiguration configuration)
        : base(configuration)
    {
    }
}