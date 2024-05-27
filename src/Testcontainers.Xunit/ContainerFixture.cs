namespace Testcontainers.Xunit;

/// <summary>
/// Fixture for sharing a container instance across multiple tests in a single class.
/// See <a href="https://xunit.net/docs/shared-context">Shared Context between Tests</a> from xUnit.net documentation for more information about fixtures.
/// A logger is automatically configured to write diagnostic messages to xUnit's <see cref="IMessageSink"/>.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public class ContainerFixture<TBuilderEntity, TContainerEntity> : IAsyncLifetime
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer
{
    private Lazy<TContainerEntity> _container;

    public ContainerFixture(IMessageSink messageSink)
    {
        MessageSink = messageSink;
        _container = new Lazy<TContainerEntity>(() =>
        {
            var containerBuilder = new TBuilderEntity().WithLogger(new MessageSinkLogger(MessageSink));
            return Configure(containerBuilder).Build();
        });
    }

    /// <summary>
    /// The message sink used for reporting diagnostic messages.
    /// </summary>
    protected IMessageSink MessageSink { get; }

    /// <summary>
    /// The container instance.
    /// </summary>
    public TContainerEntity Container => _container.Value;

    /// <summary>
    /// Extension point to further configure the container instance.
    /// </summary>
    /// <example>
    /// <code>
    /// public class MariaDbRootUserFixture(IMessageSink messageSink) : DbContainerFixture&lt;MariaDbBuilder, MariaDbContainer&gt;(messageSink)
    /// {
    ///   public override DbProviderFactory DbProviderFactory => MySqlConnectorFactory.Instance;
    ///
    ///   protected override MariaDbBuilder Configure(MariaDbBuilder builder)
    ///   {
    ///     return builder.WithUsername("root");
    ///   }
    /// }
    /// </code>
    /// </example>
    /// <param name="builder">The container builder.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected virtual TBuilderEntity Configure(TBuilderEntity builder) => builder;

    /// <inheritdoc />
    Task IAsyncLifetime.InitializeAsync() => InitializeAsync();

    /// <inheritdoc cref="IAsyncLifetime.InitializeAsync()" />
    protected virtual Task InitializeAsync() => Container.StartAsync();

    /// <inheritdoc />
    Task IAsyncLifetime.DisposeAsync() => DisposeAsync();

    /// <inheritdoc cref="IAsyncLifetime.DisposeAsync()" />
    protected virtual Task DisposeAsync() => Container.DisposeAsync().AsTask();
}