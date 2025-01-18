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

    [CanBeNull]
    private ExceptionDispatchInfo _exception;

    protected ContainerLifetime(ILogger logger)
    {
        _container = new Lazy<TContainerEntity>(() => Configure(new TBuilderEntity().WithLogger(logger)).Build());
    }

    /// <summary>
    /// Gets the container instance.
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
    LifetimeTask IAsyncLifetime.InitializeAsync() => InitializeAsync();

#if !XUNIT_V3
    /// <inheritdoc />
    LifetimeTask IAsyncLifetime.DisposeAsync() => DisposeAsyncCore();
#else
    /// <inheritdoc />
    async LifetimeTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }
#endif

    /// <summary>
    /// Extension method to further configure the container instance.
    /// </summary>
    /// <example>
    ///   <code>
    ///   public class MariaDbRootUserFixture(IMessageSink messageSink) : DbContainerFixture&lt;MariaDbBuilder, MariaDbContainer&gt;(messageSink)
    ///   {
    ///     public override DbProviderFactory DbProviderFactory =&gt; MySqlConnectorFactory.Instance;
    ///   <br />
    ///     protected override MariaDbBuilder Configure(MariaDbBuilder builder)
    ///     {
    ///       return builder.WithUsername("root");
    ///     }
    ///   }
    ///   </code>
    /// </example>
    /// <param name="builder">The container builder to configure.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected virtual TBuilderEntity Configure(TBuilderEntity builder)
    {
        return builder;
    }

    /// <inheritdoc cref="IAsyncLifetime" />
    protected virtual async LifetimeTask InitializeAsync()
    {
        try
        {
            await Container.StartAsync()
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _exception = ExceptionDispatchInfo.Capture(e);
        }
    }

    /// <inheritdoc cref="IAsyncLifetime" />
    protected virtual async LifetimeTask DisposeAsyncCore()
    {
        if (_exception == null)
        {
            await Container.DisposeAsync()
                .ConfigureAwait(false);
        }
    }
}