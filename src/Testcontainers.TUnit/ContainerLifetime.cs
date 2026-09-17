namespace Testcontainers.TUnit;

/// <summary>
/// Base class managing the lifetime of a container.
/// TUnit calls <see cref="IAsyncInitializer.InitializeAsync" /> before a test runs and <see cref="IAsyncDisposable.DisposeAsync" /> when the owning scope ends,
/// see <a href="https://tunit.dev/docs/writing-tests/lifecycle">Test Lifecycle</a> from the TUnit documentation for more information.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
public abstract class ContainerLifetime<TBuilderEntity, TContainerEntity> : IAsyncInitializer, IAsyncDisposable
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity, IContainerConfiguration>, new()
    where TContainerEntity : IContainer
{
    private readonly Lazy<TContainerEntity> _container;

    [CanBeNull]
    private ExceptionDispatchInfo _exception;

    protected ContainerLifetime(ILogger logger)
    {
        _container = new Lazy<TContainerEntity>(() => Configure().WithLogger(logger).Build());
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
    Task IAsyncInitializer.InitializeAsync() => InitializeAsync();

    /// <inheritdoc />
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Configures the container instance.
    /// </summary>
    /// <remarks>
    /// TUnit sets injected properties (e.g. <c>[ClassDataSource&lt;T&gt;]</c>) before it initializes the instance.
    /// The implementation can therefore access other injected fixtures to configure the container.
    /// </remarks>
    /// <example>
    ///   <code>
    ///   public class MariaDbRootUserFixture : DbContainerFixture&lt;MariaDbBuilder, MariaDbContainer&gt;
    ///   {
    ///     public override DbProviderFactory DbProviderFactory =&gt; MySqlConnectorFactory.Instance;
    ///   <br />
    ///     protected override MariaDbBuilder Configure()
    ///     {
    ///       return new MariaDbBuilder("mariadb:12").WithUsername("root");
    ///     }
    ///   }
    ///   </code>
    /// </example>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    protected abstract TBuilderEntity Configure();

    /// <inheritdoc cref="IAsyncInitializer.InitializeAsync" />
    protected virtual async Task InitializeAsync()
    {
        try
        {
            var cancellationToken = TestContext.Current?.Execution.CancellationToken ?? CancellationToken.None;

            await Container.StartAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _exception = ExceptionDispatchInfo.Capture(e);
        }
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_exception == null)
        {
            await Container.DisposeAsync()
                .ConfigureAwait(false);
        }
    }
}