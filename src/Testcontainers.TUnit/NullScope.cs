namespace Testcontainers.TUnit;

/// <summary>
/// A logging scope that does nothing.
/// </summary>
internal sealed class NullScope : IDisposable
{
    /// <inheritdoc />
    public void Dispose()
    {
    }
}