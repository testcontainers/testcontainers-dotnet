namespace DotNet.Testcontainers.Xunit;

/// <summary>
/// Base class for tests needing a container per test method.
/// A logger is automatically configured to write messages to xUnit's <see cref="ITestOutputHelper" />.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public abstract class ContainerTest<TBuilderEntity, TContainerEntity> : IAsyncLifetime
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer
{
    protected ContainerTest(ITestOutputHelper testOutputHelper, Func<TBuilderEntity, TBuilderEntity> configure = null)
    {
        var builder = new TBuilderEntity().WithLogger(new TestOutputLogger(testOutputHelper));
        Container = configure == null ? builder.Build() : configure(builder).Build();
    }

    /// <summary>
    /// The container instance.
    /// </summary>
    protected TContainerEntity Container { get; }

    /// <inheritdoc />
    Task IAsyncLifetime.InitializeAsync() => InitializeAsync();

    /// <inheritdoc cref="IAsyncLifetime.InitializeAsync()" />
    protected virtual Task InitializeAsync() => Container.StartAsync();

    /// <inheritdoc />
    Task IAsyncLifetime.DisposeAsync() => DisposeAsync();

    /// <inheritdoc cref="IAsyncLifetime.DisposeAsync()" />
    protected virtual Task DisposeAsync() => Container.DisposeAsync().AsTask();
}