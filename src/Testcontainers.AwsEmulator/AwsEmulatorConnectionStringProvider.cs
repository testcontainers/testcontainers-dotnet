namespace Testcontainers.AwsEmulator;

/// <summary>
/// Provides the AWS Emulator connection string.
/// </summary>
internal sealed class AwsEmulatorConnectionStringProvider : ContainerConnectionStringProvider<AwsEmulatorContainer, AwsEmulatorConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}