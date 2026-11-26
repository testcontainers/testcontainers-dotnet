namespace Testcontainers.AwsEmulator;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class AwsEmulatorConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmulatorConfiguration" /> class.
    /// </summary>
    public AwsEmulatorConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmulatorConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AwsEmulatorConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmulatorConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The container configuration.</param>
    public AwsEmulatorConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsEmulatorConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old AWS Emulator configuration.</param>
    /// <param name="newValue">The new AWS Emulator configuration.</param>
    public AwsEmulatorConfiguration(AwsEmulatorConfiguration oldValue, AwsEmulatorConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}