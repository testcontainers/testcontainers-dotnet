namespace Testcontainers.Xunit;

/// <summary>
/// Base class managing the lifetime of a container.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
public abstract class ContainerLifetime<TBuilderEntity, TContainerEntity> : IAsyncLifetime
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer
{
    private readonly Lazy<TContainerEntity> _container;
    [CanBeNull] private ExceptionDispatchInfo _exception;

    /// <summary>
    /// The logger.
    /// </summary>
    protected abstract ILogger Logger { get; }

    protected ContainerLifetime()
    {
        _container = new Lazy<TContainerEntity>(() =>
        {
            var containerBuilder = new TBuilderEntity().WithLogger(Logger);
            return Configure(containerBuilder).Build();
        });
    }

    /// <summary>
    /// Extension point to further configure the container instance.
    /// </summary>
    /// <example>
    ///   <code>
    ///   public class MariaDbRootUserFixture(IMessageSink messageSink) : DbContainerFixture&lt;MariaDbBuilder, MariaDbContainer&gt;(messageSink)
    ///   {
    ///     public override DbProviderFactory DbProviderFactory =&gt; MySqlConnectorFactory.Instance;
    ///     <br />
    ///     protected override MariaDbBuilder Configure(MariaDbBuilder builder)
    ///     {
    ///       return builder.WithUsername("root");
    ///     }
    ///   }
    ///   </code>
    /// </example>
    /// <param name="builder">The container builder.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected virtual TBuilderEntity Configure(TBuilderEntity builder) => builder;

    /// <summary>
    /// The container instance.
    /// </summary>
    public TContainerEntity Container
    {
        get
        {
            _exception?.Throw();
            return _container.Value;
        }
    }

    /// <inheritdoc />
    Task IAsyncLifetime.InitializeAsync() => InitializeAsync();

    /// <inheritdoc cref="IAsyncLifetime.InitializeAsync()" />
    protected virtual async Task InitializeAsync()
    {
        try
        {
            await Container.StartAsync();
        }
        catch (Exception e)
        {
            _exception = ExceptionDispatchInfo.Capture(e);
        }
    }

    /// <inheritdoc />
    Task IAsyncLifetime.DisposeAsync() => DisposeAsync();

    /// <inheritdoc cref="IAsyncLifetime.DisposeAsync()" />
    protected virtual async Task DisposeAsync()
    {
        if (_exception == null)
        {
            await Container.DisposeAsync();
        }
    }
}