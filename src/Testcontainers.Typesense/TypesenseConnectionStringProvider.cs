namespace Testcontainers.Typesense;

/// <summary>
/// Provides the Typesense connection string.
/// </summary>
internal sealed class TypesenseConnectionStringProvider : ContainerConnectionStringProvider<TypesenseContainer, TypesenseConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}