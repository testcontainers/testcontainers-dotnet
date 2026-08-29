namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A container that runs the Docker Compose CLI and manages the lifecycle of
  /// the Docker Compose services. It runs <c>docker compose up</c> on start and
  /// <c>docker compose down</c> on stop or dispose.
  /// </summary>
  /// <remarks>
  /// Exposed service ports
  /// (<see cref="ComposeBuilder.WithExposedService(string, ushort)" />) are
  /// proxied by an ambassador container that runs on the Docker Compose network.
  /// Use <see cref="GetServiceHost" /> and <see cref="GetServicePort" /> to
  /// access them from the test host.
  /// </remarks>
  [PublicAPI]
  public sealed class ComposeContainer : DockerContainer
  {
    /// <summary>
    /// The label that contains the Docker Compose project name.
    /// </summary>
    public const string ComposeProjectLabel = "com.docker.compose.project";

    /// <summary>
    /// The label that contains the Docker Compose service name.
    /// </summary>
    public const string ComposeServiceLabel = ComposeServiceName.ServiceLabel;

    /// <summary>
    /// The label that contains the number of the container within the Docker
    /// Compose service.
    /// </summary>
    public const string ComposeContainerNumberLabel = ComposeServiceName.ContainerNumberLabel;

    private const ushort FirstAmbassadorPort = 2000;

    private static readonly string[] ComposeConfigCommand = { "docker", "compose", "config", "--images" };

    private static readonly string[] ComposeUpCommand = { "docker", "compose", "up", "--detach" };

    private static readonly string[] ComposeDownCommand = { "docker", "compose", "down", "--volumes" };

    private static readonly IImage AmbassadorImage = new DockerImage("alpine/socat:1.8.0.3");

    private readonly ITestcontainersClient _client;

    private readonly ComposeConfiguration _configuration;

    private readonly IReadOnlyDictionary<(string ServiceName, ushort Instance, ushort Port), ushort> _ambassadorPorts;

    private IReadOnlyDictionary<(string ServiceName, ushort Instance), ComposeServiceContainer> _serviceContainers = new Dictionary<(string, ushort), ComposeServiceContainer>();

    private SocatContainer _ambassadorContainer;

    private bool _isComposeUp;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposeContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ComposeContainer(ComposeConfiguration configuration)
      : base(configuration)
    {
      _client = new TestcontainersClient(configuration.SessionId, configuration.DockerEndpointAuthConfig, configuration.Logger);
      _configuration = configuration;

      // Each exposed service port gets its own ambassador container port, starting at
      // FirstAmbassadorPort. A service port that is exposed more than once keeps the
      // ambassador container port that it got assigned first.
      var ambassadorPorts = new Dictionary<(string ServiceName, ushort Instance, ushort Port), ushort>();

      foreach (var exposedService in configuration.ExposedServices)
      {
        var service = (exposedService.ServiceName, exposedService.Instance, exposedService.Port);

        if (!ambassadorPorts.ContainsKey(service))
        {
          ambassadorPorts.Add(service, (ushort)(FirstAmbassadorPort + ambassadorPorts.Count));
        }
      }

      _ambassadorPorts = ambassadorPorts;
    }

    /// <summary>
    /// Gets the Docker Compose project name.
    /// </summary>
    public string ProjectName
    {
      get
      {
        return _configuration.ProjectName;
      }
    }

    /// <summary>
    /// Gets the host that exposes the Docker Compose service port.
    /// </summary>
    /// <remarks>
    /// The service name addresses the first instance of the service. Use
    /// <see cref="GetServiceInstanceHost" /> to address one container of a service
    /// that runs more than one.
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="servicePort">The Docker Compose service port.</param>
    /// <returns>The host that exposes the Docker Compose service port.</returns>
    public string GetServiceHost(string serviceName, ushort servicePort)
    {
      return GetServiceInstanceHost(serviceName, ComposeServiceName.FirstInstance, servicePort);
    }

    /// <summary>
    /// Gets the host that exposes the port of a Docker Compose service instance.
    /// </summary>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <param name="servicePort">The Docker Compose service port.</param>
    /// <returns>The host that exposes the Docker Compose service port.</returns>
    public string GetServiceInstanceHost(string serviceName, ushort instance, ushort servicePort)
    {
      ThrowIfServiceNotExposed(serviceName, instance, servicePort);
      return _ambassadorContainer.Hostname;
    }

    /// <summary>
    /// Gets the public host port that is mapped to the Docker Compose service port.
    /// </summary>
    /// <remarks>
    /// The service name addresses the first instance of the service. Use
    /// <see cref="GetServiceInstancePort" /> to address one container of a service
    /// that runs more than one.
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="servicePort">The Docker Compose service port.</param>
    /// <returns>The public host port that is mapped to the Docker Compose service port.</returns>
    public ushort GetServicePort(string serviceName, ushort servicePort)
    {
      return GetServiceInstancePort(serviceName, ComposeServiceName.FirstInstance, servicePort);
    }

    /// <summary>
    /// Gets the public host port that is mapped to the port of a Docker Compose
    /// service instance.
    /// </summary>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <param name="servicePort">The Docker Compose service port.</param>
    /// <returns>The public host port that is mapped to the Docker Compose service port.</returns>
    public ushort GetServiceInstancePort(string serviceName, ushort instance, ushort servicePort)
    {
      ThrowIfServiceNotExposed(serviceName, instance, servicePort);
      return _ambassadorContainer.GetMappedPublicPort(_ambassadorPorts[(serviceName, instance, servicePort)]);
    }

    /// <summary>
    /// Gets the container that belongs to the Docker Compose service.
    /// </summary>
    /// <remarks>
    /// Docker Compose manages the lifecycle of the container. Stopping or disposing
    /// the returned container does not affect it. Stop or dispose the
    /// <see cref="ComposeContainer" /> instead.
    ///
    /// The service name addresses the first instance of the service. Use
    /// <see cref="GetServiceInstanceContainer" /> to address one container of a
    /// service that runs more than one.
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <returns>The container that belongs to the Docker Compose service.</returns>
    /// <exception cref="ComposeServiceNotFoundException">The Docker Compose service was not found.</exception>
    public IContainer GetServiceContainer(string serviceName)
    {
      return GetServiceInstanceContainer(serviceName, ComposeServiceName.FirstInstance);
    }

    /// <summary>
    /// Gets the container that belongs to a Docker Compose service instance.
    /// </summary>
    /// <remarks>
    /// Docker Compose manages the lifecycle of the container. Stopping or disposing
    /// the returned container does not affect it. Stop or dispose the
    /// <see cref="ComposeContainer" /> instead.
    /// </remarks>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <returns>The container that belongs to the Docker Compose service instance.</returns>
    /// <exception cref="ComposeServiceNotFoundException">The Docker Compose service was not found.</exception>
    public IContainer GetServiceInstanceContainer(string serviceName, ushort instance)
    {
      if (_serviceContainers.TryGetValue((serviceName, instance), out var serviceContainer))
      {
        return serviceContainer;
      }

      throw new ComposeServiceNotFoundException(ComposeServiceName.GetDisplayName(serviceName, instance), ProjectName);
    }

    /// <inheritdoc />
    protected override async Task UnsafeStartAsync(CancellationToken ct = default)
    {
      await base.UnsafeStartAsync(ct)
        .ConfigureAwait(false);

      await RegisterProjectFilterAsync(ct)
        .ConfigureAwait(false);

      await PullImagesAsync(ct)
        .ConfigureAwait(false);

      await ComposeUpAsync(ct)
        .ConfigureAwait(false);

      var composeContainers = await GetComposeContainersAsync(ct)
        .ConfigureAwait(false);

      ThrowIfServiceNotFound(composeContainers);

      await StartAmbassadorContainerAsync(composeContainers, ct)
        .ConfigureAwait(false);

      _serviceContainers = composeContainers
        .ToDictionary(composeContainer => composeContainer.Key, composeContainer => CreateServiceContainer(composeContainer.Key, composeContainer.Value));

      await ThrowIfAnyServiceFailedAsync(_serviceContainers
          .Select(serviceContainer => GetOperationFailureAsync(serviceContainer.Key, serviceContainer.Value.AttachAsync(ct), ct)))
        .ConfigureAwait(false);

      // Group the service containers by whether they already exited or not.
      // Docker Compose does not fail if a service exits immediately.
      var serviceContainersByExited = _serviceContainers
        .ToLookup(serviceContainer => TestcontainersStates.Exited.Equals(serviceContainer.Value.State));

      // Check the exit code of the services that already exited first. It fails faster
      // and reports the cause more accurately than the readiness checks of the
      // services that depend on them.
      await ThrowIfAnyServiceFailedAsync(serviceContainersByExited[true]
          .Select(serviceContainer => GetExitCodeFailureAsync(serviceContainer.Key, serviceContainer.Value.Id, ct)))
        .ConfigureAwait(false);

      // Docker Compose already started the services, the service containers run the
      // readiness checks only. A service that ran to completion, like a one-shot
      // migration service, does not become ready anymore. Every other state, such as
      // a service that is still restarting, does run its readiness check.
      await ThrowIfAnyServiceFailedAsync(serviceContainersByExited[false]
          .Select(serviceContainer => GetOperationFailureAsync(serviceContainer.Key, serviceContainer.Value.StartAsync(ct), ct)))
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeStopAsync(CancellationToken ct = default)
    {
      await ComposeDownAsync(ct)
        .ConfigureAwait(false);

      await base.UnsafeStopAsync(ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
      await ComposeDownAsync(ct)
        .ConfigureAwait(false);

      await base.UnsafeDeleteAsync(ct)
        .ConfigureAwait(false);
    }

    /// <summary>
    /// Registers the Docker Compose project label with the Resource Reaper. The
    /// Resource Reaper removes the Docker resources that belong to the project when
    /// the test process does not clean them up (e.g. when it crashes).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the filter has been registered.</returns>
    private async Task RegisterProjectFilterAsync(CancellationToken ct = default)
    {
      if (!TestcontainersSettings.ResourceReaperEnabled || !ResourceReaper.DefaultSessionId.Equals(_configuration.SessionId))
      {
        return;
      }

      var isWindowsEngineEnabled = await _client.System.GetIsWindowsEngineEnabled(ct)
        .ConfigureAwait(false);

      var resourceReaper = await ResourceReaper.GetAndStartDefaultAsync(_configuration.DockerEndpointAuthConfig, Logger, isWindowsEngineEnabled, ct)
        .ConfigureAwait(false);

      if (resourceReaper != null)
      {
        await resourceReaper.RegisterFilterAsync($"label={ComposeProjectLabel}={ProjectName}", ct)
          .ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Pulls the images of the Docker Compose services.
    /// </summary>
    /// <remarks>
    /// Docker Compose runs inside a container and does not have access to the
    /// Docker configuration of the test host. Pull the images from the test host
    /// instead, so that its Docker credentials and credential helpers apply. Docker
    /// Compose pulls the images that are still missing itself.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the images have been pulled.</returns>
    private async Task PullImagesAsync(CancellationToken ct = default)
    {
      if (false.Equals(_configuration.Pull))
      {
        return;
      }

      IEnumerable<string> images;

      try
      {
        var execResult = await ExecAsync(ComposeConfigCommand, ct)
          .ThrowOnFailure()
          .ConfigureAwait(false);

        images = execResult.Stdout.Split('\n')
          .Select(image => image.Trim())
          .Where(image => image.Length > 0)
          .Distinct();
      }
      catch (Exception e)
      {
        // Do not fail the start. Resolving the images is best-effort, the Docker
        // Compose up command reports an invalid configuration itself.
        Logger.DockerComposeResolveImagesFailed(ProjectName, e);
        return;
      }

      await Task.WhenAll(images.Select(image => PullImageAsync(image, ct)))
        .ConfigureAwait(false);
    }

    /// <summary>
    /// Pulls the image of a Docker Compose service if it is not present on the
    /// Docker host.
    /// </summary>
    /// <param name="image">The image of the Docker Compose service.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the image has been pulled.</returns>
    private async Task PullImageAsync(string image, CancellationToken ct = default)
    {
      try
      {
        var imageExists = await _client.Image.ExistsWithIdAsync(image, ct)
          .ConfigureAwait(false);

        if (imageExists)
        {
          return;
        }

        await _client.PullImageAsync(new DockerImage(image), ct)
          .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        // A service that Docker Compose builds does not have an image to pull yet. Do
        // not fail the start, Docker Compose reports a missing image itself.
        Logger.DockerComposePullImageFailed(image, e);
      }
    }

    /// <summary>
    /// Creates and starts the Docker Compose services (<c>docker compose up</c>).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker Compose services have been started.</returns>
    private async Task ComposeUpAsync(CancellationToken ct = default)
    {
      var upCommand = new List<string>(ComposeUpCommand);

      foreach (var scaledService in _configuration.ScaledServices)
      {
        upCommand.Add("--scale");
        upCommand.Add($"{scaledService.Key}={scaledService.Value.ToString(CultureInfo.InvariantCulture)}");
      }

      // Docker Compose starts every service if no service is set. Only when the
      // services are restricted do the scaled services have to be named as well,
      // otherwise Docker Compose does not start them.
      if (_configuration.Services.Any())
      {
        upCommand.AddRange(_configuration.Services.Concat(_configuration.ScaledServices.Keys).Distinct());
      }

      // A failed `up` can leave partially created Docker resources behind that `down`
      // removes. Set the flag before running the command.
      _isComposeUp = true;

      var execResult = await ExecAsync(upCommand, ct)
        .ConfigureAwait(false);

      if (0L.Equals(execResult.ExitCode))
      {
        return;
      }

      // Docker Compose fails the command if a service that another service depends on
      // did not complete successfully. It reports the exit code of the service, but
      // not its logs. Report the service that caused the failure instead of the
      // Docker Compose command that failed.
      var composeContainers = await GetComposeContainersAsync(ct)
        .ConfigureAwait(false);

      await ThrowIfServiceExitedUnsuccessfullyAsync(composeContainers, ct)
        .ConfigureAwait(false);

      throw new ExecFailedException(execResult);
    }

    /// <summary>
    /// Stops and removes the Docker Compose services (<c>docker compose down</c>).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Docker Compose services have been removed.</returns>
    private async Task ComposeDownAsync(CancellationToken ct = default)
    {
      if (!_isComposeUp)
      {
        return;
      }

      _isComposeUp = false;

      _serviceContainers = new Dictionary<(string, ushort), ComposeServiceContainer>();

      try
      {
        // Remove the ambassador container before the Docker Compose networks. Docker
        // does not remove networks that still have containers connected.
        if (_ambassadorContainer != null)
        {
          await _ambassadorContainer.DisposeAsync()
            .ConfigureAwait(false);
        }
      }
      catch (Exception e)
      {
        Logger.DockerComposeDownFailed(ProjectName, e);
      }
      finally
      {
        _ambassadorContainer = null;
      }

      try
      {
        _ = await ExecAsync(ComposeDownCommand, ct)
          .ThrowOnFailure()
          .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        // Do not throw if the cleanup fails. The Resource Reaper removes the remaining
        // Docker resources that belong to the project.
        Logger.DockerComposeDownFailed(ProjectName, e);
      }
    }

    /// <summary>
    /// Gets the containers that belong to the Docker Compose project.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the containers have been listed, returning the containers indexed by their service instance.</returns>
    private async Task<IReadOnlyDictionary<(string ServiceName, ushort Instance), ContainerListResponse>> GetComposeContainersAsync(CancellationToken ct = default)
    {
      var filters = new FilterByProperty().Add("label", $"{ComposeProjectLabel}={ProjectName}");

      var containers = await _client.Container.GetAllAsync(filters, ct)
        .ConfigureAwait(false);

      // Each container of a Docker Compose service is one instance, e.g. web-1 and
      // web-2. Do not collapse them, the instances are addressed individually.
      return containers
        .Select(container => (Service: ComposeServiceName.GetInstance(container.Labels), Container: container))
        .Where(item => item.Service.HasValue)
        .ToDictionary(item => item.Service.Value, item => item.Container);
    }

    /// <summary>
    /// Starts the ambassador container that proxies the exposed Docker Compose
    /// service ports.
    /// </summary>
    /// <param name="composeContainers">The Docker Compose containers indexed by their service instance.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the ambassador container has been started.</returns>
    private async Task StartAmbassadorContainerAsync(IReadOnlyDictionary<(string ServiceName, ushort Instance), ContainerListResponse> composeContainers, CancellationToken ct = default)
    {
      if (_ambassadorPorts.Count == 0)
      {
        return;
      }

      // Target the IP address of the container instead of the service name. The
      // service name resolves to any container of the service, which does not address
      // a single instance, and the container name is not guaranteed to be a
      // resolvable DNS name. It exceeds the 63 character label limit for a long
      // Docker Compose project name.
      var serviceIpAddresses = _ambassadorPorts.Keys
        .Select(ambassadorPort => (ambassadorPort.ServiceName, ambassadorPort.Instance))
        .Distinct()
        .ToDictionary(service => service, service => GetContainerIpAddress(composeContainers[service]));

      // A service that ran to completion does not have an IP address anymore. Neither
      // does a service that shares another container's network namespace, e.g.
      // `network_mode: service:app`, it is not attached to a network itself. The
      // ambassador container cannot reach such a service.
      var unreachableServices = serviceIpAddresses
        .Where(serviceIpAddress => serviceIpAddress.Value == null)
        .Select(serviceIpAddress => serviceIpAddress.Key)
        .ToArray();

      if (unreachableServices.Length > 0)
      {
        // An exposed service that exited unsuccessfully is the actual cause. Report its
        // exit code and logs instead of the ambassador container that cannot reach it.
        await ThrowIfServiceExitedUnsuccessfullyAsync(composeContainers.Where(composeContainer => unreachableServices.Contains(composeContainer.Key)), ct)
          .ConfigureAwait(false);

        var serviceNames = unreachableServices
          .Select(service => ComposeServiceName.GetDisplayName(service.ServiceName, service.Instance));

        throw new InvalidOperationException($"The exposed Docker Compose service(s) '{string.Join("', '", serviceNames)}' do not have an IP address. Exposing a service port requires the service to run and to be attached to a Docker network that the ambassador container can join.");
      }

      // The ambassador container shares the Resource Reaper session of the Docker
      // Compose container. The session is not necessarily the default one, disabling
      // the cleanup sets an empty session id that the Resource Reaper ignores.
      var socatBuilder = new SocatBuilder(AmbassadorImage)
        .WithDockerEndpoint(_configuration.DockerEndpointAuthConfig)
        .WithLabel(ResourceReaper.ResourceReaperSessionLabel, _configuration.SessionId.ToString("D"))
        .WithLogger(Logger);

      _ambassadorContainer = _ambassadorPorts
        .Aggregate(socatBuilder, (builder, ambassadorPort) => builder.WithTarget(ambassadorPort.Value, serviceIpAddresses[(ambassadorPort.Key.ServiceName, ambassadorPort.Key.Instance)], ambassadorPort.Key.Port))
        .Build();

      await _ambassadorContainer.StartAsync(ct)
        .ConfigureAwait(false);

      // Docker Compose usually attaches each service to at least one network. The
      // ambassador container connects to them after it started. Socat resolves the
      // target when it accepts a connection, not when it starts, and the readiness
      // check of the exposed service ports runs after this.
      var networks = serviceIpAddresses.Keys
        .SelectMany(service => composeContainers[service].NetworkSettings.Networks.Keys)
        .Distinct();

      foreach (var network in networks)
      {
        await _ambassadorContainer.ConnectAsync(network, ct)
          .ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Creates a container that attaches to an existing Docker Compose service
    /// container.
    /// </summary>
    /// <param name="service">The Docker Compose service instance.</param>
    /// <param name="container">The Docker Compose container.</param>
    /// <returns>The container that attaches to the Docker Compose service container.</returns>
    private ComposeServiceContainer CreateServiceContainer((string ServiceName, ushort Instance) service, ContainerListResponse container)
    {
      var ipAddress = GetContainerIpAddress(container);

      var ambassadorPorts = _ambassadorPorts
        .Where(ambassadorPort => ambassadorPort.Key.ServiceName == service.ServiceName && ambassadorPort.Key.Instance == service.Instance)
        .ToDictionary(ambassadorPort => ambassadorPort.Key.Port, ambassadorPort => ambassadorPort.Value);

      // Wait until the exposed service ports accept connections. The check runs
      // inside the ambassador container, which keeps it independent of the service
      // image.
      var servicePortWaitStrategies = ambassadorPorts.Keys
        .Select(servicePort => new WaitStrategy(new UntilComposeServicePortIsAvailable(_ambassadorContainer, ipAddress, servicePort)));

      var serviceReadinessWaitStrategies = _configuration.ServiceReadiness
        .Where(serviceReadiness => serviceReadiness.ServiceName == service.ServiceName && serviceReadiness.Instance == service.Instance)
        .SelectMany(serviceReadiness => serviceReadiness.WaitStrategies);

      // The exposed service ports accept connections before the wait strategies of
      // the service run. Materialize them, the container configuration outlives this
      // method and would create new wait strategies on every enumeration.
      var waitStrategies = servicePortWaitStrategies
        .Concat(serviceReadinessWaitStrategies)
        .ToArray();

      // The cast selects the constructor overload that copies the resource
      // configuration (Docker endpoint, session id, logger) and intentionally drops
      // the Docker Compose container configuration. Inheriting it would create the
      // Docker Compose container's networks and restart its dependent containers once
      // per service.
      var resourceConfiguration = new ContainerConfiguration(
        (IResourceConfiguration<CreateContainerParameters>)_configuration);

      var containerConfiguration = new ContainerConfiguration(
        image: GetServiceImage(container),
        outputConsumer: Consume.DoNotConsumeStdoutAndStderr(),
        waitStrategies: waitStrategies,
        startupCallback: (_, _, _) => Task.CompletedTask);

      var serviceConfiguration = new ContainerConfiguration(
        resourceConfiguration,
        containerConfiguration);

      return new ComposeServiceContainer(serviceConfiguration, container.ID, _ambassadorContainer, ambassadorPorts);
    }

    /// <summary>
    /// Throws an exception when one or more Docker Compose services failed.
    /// </summary>
    /// <remarks>
    /// Gather the failure of every service before reporting it. Docker Compose runs
    /// the services at the same time, and more than one of them can fail.
    /// </remarks>
    /// <param name="serviceFailures">The gathered failure of each Docker Compose service instance.</param>
    /// <returns>Task that completes when the failures have been gathered.</returns>
    /// <exception cref="ComposeServiceFailedException">One or more Docker Compose services failed.</exception>
    private async Task ThrowIfAnyServiceFailedAsync(IEnumerable<Task<KeyValuePair<string, Exception>>> serviceFailures)
    {
      var services = await Task.WhenAll(serviceFailures)
        .ConfigureAwait(false);

      var failures = services
        .Where(service => service.Value != null)
        .ToArray();

      if (failures.Length > 0)
      {
        throw new ComposeServiceFailedException(ProjectName, failures);
      }
    }

    /// <summary>
    /// Throws an exception when one or more Docker Compose services that already
    /// exited have an unsuccessful exit code.
    /// </summary>
    /// <param name="composeContainers">The Docker Compose containers indexed by their service instance.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the exit codes have been checked.</returns>
    /// <exception cref="ComposeServiceFailedException">One or more Docker Compose services exited unsuccessfully.</exception>
    private Task ThrowIfServiceExitedUnsuccessfullyAsync(IEnumerable<KeyValuePair<(string ServiceName, ushort Instance), ContainerListResponse>> composeContainers, CancellationToken ct = default)
    {
      var exitedContainers = composeContainers
        .Where(composeContainer => nameof(TestcontainersStates.Exited).Equals(composeContainer.Value.State, StringComparison.OrdinalIgnoreCase));

      return ThrowIfAnyServiceFailedAsync(exitedContainers
        .Select(composeContainer => GetExitCodeFailureAsync(composeContainer.Key, composeContainer.Value.ID, ct)));
    }

    /// <summary>
    /// Gets the failure of a Docker Compose service that exited unsuccessfully.
    /// </summary>
    /// <param name="service">The Docker Compose service instance.</param>
    /// <param name="id">The id of the Docker Compose container.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the exit code has been checked, returning the failure of the service, or <c>null</c> if it succeeded.</returns>
    private async Task<KeyValuePair<string, Exception>> GetExitCodeFailureAsync((string ServiceName, ushort Instance) service, string id, CancellationToken ct = default)
    {
      var serviceName = ComposeServiceName.GetDisplayName(service.ServiceName, service.Instance);

      var exitCode = await _client.GetContainerExitCodeAsync(id, ct)
        .ConfigureAwait(false);

      if (exitCode == 0)
      {
        return new KeyValuePair<string, Exception>(serviceName, null);
      }

      var (stdout, stderr) = await _client.GetContainerLogsAsync(id, ct: ct)
        .ConfigureAwait(false);

      return new KeyValuePair<string, Exception>(serviceName, new ContainerNotRunningException(id, stdout, stderr, exitCode, null));
    }

    /// <summary>
    /// Gets the failure of an operation that a Docker Compose service ran.
    /// </summary>
    /// <remarks>
    /// An operation that starts or attaches a container reports its failure by
    /// throwing. Capture it, so that the failures of all services can be gathered
    /// before one of them is reported.
    /// </remarks>
    /// <param name="service">The Docker Compose service instance.</param>
    /// <param name="operation">The operation that the Docker Compose service ran.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the operation has run, returning its failure, or <c>null</c> if the operation succeeded.</returns>
    private static async Task<KeyValuePair<string, Exception>> GetOperationFailureAsync((string ServiceName, ushort Instance) service, Task operation, CancellationToken ct = default)
    {
      var serviceName = ComposeServiceName.GetDisplayName(service.ServiceName, service.Instance);

      try
      {
        await operation
          .ConfigureAwait(false);

        return new KeyValuePair<string, Exception>(serviceName, null);
      }
      catch (Exception e) when (!ct.IsCancellationRequested)
      {
        return new KeyValuePair<string, Exception>(serviceName, e);
      }
    }

    /// <summary>
    /// Throws an exception when a Docker Compose service that the configuration
    /// references does not belong to the Docker Compose project.
    /// </summary>
    /// <param name="composeContainers">The Docker Compose containers indexed by their service instance.</param>
    /// <exception cref="ComposeServiceNotFoundException">The Docker Compose service was not found.</exception>
    private void ThrowIfServiceNotFound(IReadOnlyDictionary<(string ServiceName, ushort Instance), ContainerListResponse> composeContainers)
    {
      // A wait strategy or exposed port that references a service that does not exist
      // would otherwise pass silently, e.g. when the service name contains a typo.
      var serviceNotFound = _configuration.ExposedServices
        .Select(exposedService => (exposedService.ServiceName, exposedService.Instance))
        .Concat(_configuration.ServiceReadiness.Select(serviceReadiness => (serviceReadiness.ServiceName, serviceReadiness.Instance)))
        .Where(service => !composeContainers.ContainsKey(service))
        .Select(service => ComposeServiceName.GetDisplayName(service.ServiceName, service.Instance))
        .FirstOrDefault();

      if (serviceNotFound != null)
      {
        throw new ComposeServiceNotFoundException(serviceNotFound, ProjectName);
      }
    }

    /// <summary>
    /// Throws an exception when the Docker Compose service port is not exposed or
    /// when the ambassador container is not running.
    /// </summary>
    /// <param name="serviceName">The Docker Compose service name.</param>
    /// <param name="instance">The number of the container within the Docker Compose service.</param>
    /// <param name="servicePort">The Docker Compose service port.</param>
    /// <exception cref="ComposeServiceNotExposedException">The Docker Compose service port is not exposed.</exception>
    /// <exception cref="ComposeServiceNotFoundException">The Docker Compose services have not been started.</exception>
    private void ThrowIfServiceNotExposed(string serviceName, ushort instance, ushort servicePort)
    {
      if (!_ambassadorPorts.ContainsKey((serviceName, instance, servicePort)))
      {
        throw new ComposeServiceNotExposedException(ComposeServiceName.GetDisplayName(serviceName, instance), servicePort);
      }

      if (_ambassadorContainer == null)
      {
        throw new ComposeServiceNotFoundException(ComposeServiceName.GetDisplayName(serviceName, instance), ProjectName);
      }
    }

    /// <summary>
    /// Gets the IP address of a Docker Compose container.
    /// </summary>
    /// <param name="container">The Docker Compose container.</param>
    /// <returns>The IP address of the Docker Compose container on the first network it is attached to, or <c>null</c> if it does not have one.</returns>
    private static string GetContainerIpAddress(ContainerListResponse container)
    {
      return container.NetworkSettings.Networks.Values
        .Select(network => network.IPAddress)
        .FirstOrDefault(ipAddress => !string.IsNullOrEmpty(ipAddress));
    }

    /// <summary>
    /// Gets the image of the Docker Compose service container.
    /// </summary>
    /// <param name="container">The Docker Compose container.</param>
    /// <returns>The image of the Docker Compose service container, or <c>null</c> if the image reference cannot be parsed.</returns>
    private static IImage GetServiceImage(ContainerListResponse container)
    {
      try
      {
        return new DockerImage(container.Image);
      }
      catch (ArgumentException)
      {
        // The image reference comes from the Docker daemon and is not necessarily a
        // valid repository/tag reference, e.g. when the service runs an untagged image.
        // The image is informational only, do not fail the start.
        return null;
      }
    }
  }
}
