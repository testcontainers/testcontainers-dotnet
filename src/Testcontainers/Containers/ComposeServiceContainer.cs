namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;

  /// <summary>
  /// A container that attaches to an existing container that Docker Compose
  /// created, instead of creating a new one.
  /// </summary>
  /// <remarks>
  /// If the service port is exposed through the ambassador container, the port
  /// members resolve to the ambassador container's mapped port instead of the
  /// service container's (usually unbound) port. This allows wait strategies to
  /// run against Docker Compose services like against any other container.
  ///
  /// Docker Compose owns the lifecycle of the container. The members that create,
  /// start, stop or remove a container do not affect it. Only
  /// <see cref="ComposeContainer" /> starts and removes the Docker Compose
  /// services.
  /// </remarks>
  internal sealed class ComposeServiceContainer : DockerContainer
  {
    /// <summary>
    /// The protocol that the ambassador container proxies.
    /// </summary>
    private const string TcpProtocol = "tcp";

    private readonly string _containerId;

    private readonly SocatContainer _ambassadorContainer;

    private readonly IReadOnlyDictionary<ushort, ushort> _ambassadorPorts;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeServiceContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="containerId">The id of the existing container that Docker Compose created.</param>
    /// <param name="ambassadorContainer">The ambassador container that proxies the exposed service ports, or <c>null</c> if no service port is exposed.</param>
    /// <param name="ambassadorPorts">A dictionary that maps the exposed service ports to the ambassador container ports.</param>
    public ComposeServiceContainer(IContainerConfiguration configuration, string containerId, SocatContainer ambassadorContainer, IReadOnlyDictionary<ushort, ushort> ambassadorPorts)
      : base(configuration)
    {
      _containerId = containerId;
      _ambassadorContainer = ambassadorContainer;
      _ambassadorPorts = ambassadorPorts;
    }

    /// <inheritdoc />
    public override ushort GetMappedPublicPort(string containerPort)
    {
      // The container port may include the protocol (e.g. 80/tcp). The ambassador
      // container proxies TCP only; leave every other protocol to the base
      // implementation, which resolves the port that Docker Compose published.
      var port = containerPort == null ? Array.Empty<string>() : containerPort.Split('/');

      var isTcpPort = port.Length == 1 || (port.Length == 2 && TcpProtocol.Equals(port[1], StringComparison.OrdinalIgnoreCase));

      if (_ambassadorContainer != null && isTcpPort && ushort.TryParse(port[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var servicePort) && _ambassadorPorts.TryGetValue(servicePort, out var ambassadorPort))
      {
        return _ambassadorContainer.GetMappedPublicPort(ambassadorPort);
      }

      return base.GetMappedPublicPort(containerPort);
    }

    /// <inheritdoc />
    public override IReadOnlyDictionary<ushort, ushort> GetMappedPublicPorts()
    {
      var mappedPublicPorts = base.GetMappedPublicPorts().ToDictionary(item => item.Key, item => item.Value);

      if (_ambassadorContainer == null)
      {
        return mappedPublicPorts;
      }

      // The exposed service ports take precedence over the ports that Docker Compose
      // published. They are reachable from the test host either way, but only the
      // ambassador container port is covered by the service's readiness check.
      foreach (var ambassadorPort in _ambassadorPorts)
      {
        mappedPublicPorts[ambassadorPort.Key] = _ambassadorContainer.GetMappedPublicPort(ambassadorPort.Value);
      }

      return mappedPublicPorts;
    }

    /// <summary>
    /// Attaches to the container that Docker Compose created.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the container has been attached.</returns>
    public async Task AttachAsync(CancellationToken ct = default)
    {
      using var disposable = await AcquireLockAsync(ct)
        .ConfigureAwait(false);

      await UnsafeCreateAsync(ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Docker Compose already created the container. Attach to it instead of
    /// creating a new one.
    /// </remarks>
    protected override Task<string> UnsafeCreateContainerAsync(CancellationToken ct = default)
    {
      return Task.FromResult(_containerId);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Docker Compose already started the container. Starting it again would run a
    /// service that ran to completion, such as a one-shot migration service, a
    /// second time. Run the readiness checks only.
    /// </remarks>
    protected override Task UnsafeStartContainerAsync(CancellationToken ct = default)
    {
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Docker Compose owns the lifecycle of the container. Stopping it behind
    /// Docker Compose's back would break the services that depend on it.
    /// </remarks>
    protected override Task UnsafeStopAsync(CancellationToken ct = default)
    {
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Docker Compose owns the lifecycle of the container. Removing it behind
    /// Docker Compose's back would break the services that depend on it, and leave
    /// <c>docker compose down</c> with a container that does not exist anymore.
    /// </remarks>
    protected override Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
      return Task.CompletedTask;
    }
  }
}
