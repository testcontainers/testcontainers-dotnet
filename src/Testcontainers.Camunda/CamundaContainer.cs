namespace Testcontainers.Camunda;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CamundaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CamundaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public CamundaContainer(IContainerConfiguration configuration) : base(configuration)
    {
    }
}