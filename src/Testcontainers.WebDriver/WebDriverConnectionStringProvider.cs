namespace Testcontainers.WebDriver;

/// <summary>
/// Provides the WebDriver connection string.
/// </summary>
internal sealed class WebDriverConnectionStringProvider : ContainerConnectionStringProvider<WebDriverContainer, WebDriverConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}