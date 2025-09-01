namespace Testcontainers.Mockaco;

public class MockacoConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MockacoConfiguration" /> class.
    /// </summary>
    /// <param name="templatesPath">The path wiches contain mock templates.</param>
    public MockacoConfiguration(string templatesPath = null)
    {
        TemplatesPath = templatesPath ?? "/Mocks/Templates";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockacoConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MockacoConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockacoConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MockacoConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockacoConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MockacoConfiguration(MockacoConfiguration resourceConfiguration)
        : this(new MockacoConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockacoConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MockacoConfiguration(MockacoConfiguration oldValue, MockacoConfiguration newValue)
        : base(oldValue, newValue)
    {
        TemplatesPath = BuildConfiguration.Combine(oldValue?.TemplatesPath, newValue?.TemplatesPath);
    }

    /// <summary>
    /// Gets the path to the mock`s template file.
    /// </summary>
    public string TemplatesPath { get; }
}