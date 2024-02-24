namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Networks;
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker container builder.
  /// </summary>
  /// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
  /// <typeparam name="TContainerEntity">The resource entity.</typeparam>
  [PublicAPI]
  public interface IContainerBuilder<out TBuilderEntity, out TContainerEntity> : IAbstractBuilder<TBuilderEntity, TContainerEntity, CreateContainerParameters>
  {
    /// <summary>
    /// Sets the dependent container to resolve and start before starting this container configuration.
    /// </summary>
    /// <param name="container">The dependent container.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity DependsOn(IContainer container);

    /// <summary>
    /// Sets the dependent network to resolve and create before starting this container configuration.
    /// </summary>
    /// <param name="network">The dependent network.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity DependsOn(INetwork network);

    /// <summary>
    /// Sets the dependent volume to resolve and create before starting this container configuration.
    /// </summary>
    /// <param name="volume">The dependent volume.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity DependsOn(IVolume volume, string destination);

    /// <summary>
    /// Sets the dependent volume to resolve and create before starting this container configuration.
    /// </summary>
    /// <param name="volume">The dependent volume.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <param name="accessMode">The volume access mode.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity DependsOn(IVolume volume, string destination, AccessMode accessMode);

    /// <summary>
    /// Sets an image for which to create the container.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithImage(string image);

    /// <summary>
    /// Sets an image for which to create the container.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithImage(IImage image);

    /// <summary>
    /// Sets the image pull policy.
    /// </summary>
    /// <param name="imagePullPolicy">The image pull policy.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithImagePullPolicy(Func<ImageInspectResponse, bool> imagePullPolicy);

    /// <summary>
    /// Sets the name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithName(string name);

    /// <summary>
    /// Sets the hostname.
    /// </summary>
    /// <param name="hostname">The hostname.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithHostname(string hostname);

    /// <summary>
    /// Sets the MAC address.
    /// </summary>
    /// <param name="macAddress">The MAC address.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithMacAddress(string macAddress);

    /// <summary>
    /// Sets the working directory.
    /// </summary>
    /// <param name="workingDirectory">The working directory.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithWorkingDirectory(string workingDirectory);

    /// <summary>
    /// Overrides the container's entrypoint executable.
    /// </summary>
    /// <param name="entrypoint">The entrypoint executable.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithEntrypoint(params string[] entrypoint);

    /// <summary>
    /// Overrides the container's command arguments.
    /// </summary>
    /// <param name="command">A list of commands, "executable", "param1", "param2" or "param1", "param2".</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithCommand(params string[] command);

    /// <summary>
    /// Sets the environment variable.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <param name="value">The environment variable value.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithEnvironment(string name, string value);

    /// <summary>
    /// Sets the environment variable.
    /// </summary>
    /// <param name="environments">A dictionary of environment variables.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithEnvironment(IReadOnlyDictionary<string, string> environments);

    /// <summary>
    /// Exposes the port without publishing it to the host system's interfaces.
    /// </summary>
    /// <param name="port">The port.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithExposedPort(int port);

    /// <summary>
    /// Exposes the port without publishing it to the host system's interfaces.
    /// </summary>
    /// <param name="port">The port.</param>
    /// <remarks>Append /tcp|udp|sctp to <paramref name="port" /> to change the protocol e.g. "53/udp".</remarks>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithExposedPort(string port);

    /// <summary>
    /// Binds the container port to a random host port.
    /// </summary>
    /// <param name="port">The container port.</param>
    /// <param name="assignRandomHostPort">Determines whether Testcontainers assigns a random host port or not.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithPortBinding(int port, bool assignRandomHostPort = false);

    /// <summary>
    /// Binds the container port to a specific host port.
    /// </summary>
    /// <param name="hostPort">The host port.</param>
    /// <param name="containerPort">The container port.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithPortBinding(int hostPort, int containerPort);

    /// <summary>
    /// Binds the container port to a random host port.
    /// </summary>
    /// <param name="port">The container port.</param>
    /// <param name="assignRandomHostPort">Determines whether Testcontainers assigns a random host port or not.</param>
    /// <remarks>Append /tcp|udp|sctp to <paramref name="port" /> to change the protocol e.g. "53/udp".</remarks>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithPortBinding(string port, bool assignRandomHostPort = false);

    /// <summary>
    /// Binds the container port to a specific host port.
    /// </summary>
    /// <param name="hostPort">The host port.</param>
    /// <param name="containerPort">The container port.</param>
    /// <remarks>Append /tcp|udp|sctp to <paramref name="containerPort" /> to change the protocol e.g. "53/udp".</remarks>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithPortBinding(string hostPort, string containerPort);

    /// <summary>
    /// Copies the byte array content of the resource mapping to the created container before it starts.
    /// </summary>
    /// <param name="resourceMapping">The resource mapping.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithResourceMapping(IResourceMapping resourceMapping);

    /// <summary>
    /// Copies the byte array content to the created container before it starts.
    /// </summary>
    /// <param name="resourceContent">The byte array content of the resource mapping.</param>
    /// <param name="filePath">The target file path to copy the file to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithResourceMapping(byte[] resourceContent, string filePath, UnixFileModes fileMode = Unix.FileMode644);

    /// <summary>
    /// Copies the contents of a URL, a test host directory or file to the container before it starts.
    /// </summary>
    /// <remarks>
    /// If the source corresponds to a file or the Uri scheme corresponds to a file,
    /// the content is copied to the target directory path. If the Uri scheme
    /// corresponds to HTTP or HTTPS, the content is copied to the target file path.
    ///
    /// If you prefer to copy a file to a specific target file path instead of a
    /// directory, use: <see cref="WithResourceMapping(FileInfo, FileInfo, UnixFileModes)" />.
    /// </remarks>
    /// <param name="source">The source URL, directory or file to be copied.</param>
    /// <param name="target">The target directory or file path to copy the file to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithResourceMapping(string source, string target, UnixFileModes fileMode = Unix.FileMode644);

    /// <summary>
    /// Copies a test host directory or file to the container before it starts.
    /// </summary>
    /// <param name="source">The source directory to be copied.</param>
    /// <param name="target">The target directory path to copy the files to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithResourceMapping(DirectoryInfo source, string target, UnixFileModes fileMode = Unix.FileMode644);

    /// <summary>
    /// Copies a test host directory or file to the container before it starts.
    /// </summary>
    /// <param name="source">The source file to be copied.</param>
    /// <param name="target">The target directory path to copy the file to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithResourceMapping(FileInfo source, string target, UnixFileModes fileMode = Unix.FileMode644);

    /// <summary>
    /// Copies a test host file to the container before it starts.
    /// </summary>
    /// <param name="source">The source file to be copied.</param>
    /// <param name="target">The target file path to copy the file to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithResourceMapping(FileInfo source, FileInfo target, UnixFileModes fileMode = Unix.FileMode644);

    /// <summary>
    /// Copies a file from a URL to the container before it starts.
    /// </summary>
    /// <remarks>
    /// If the Uri scheme corresponds to a file, the content is copied to the target
    /// directory path. If the Uri scheme corresponds to HTTP or HTTPS, the content is
    /// copied to the target file path.
    ///
    /// The Uri scheme must be either <c>http</c>, <c>https</c> or <c>file</c>.
    ///
    /// If you prefer to copy a file to a specific target file path instead of a
    /// directory, use: <see cref="WithResourceMapping(FileInfo, FileInfo, UnixFileModes)" />.
    /// </remarks>
    /// <param name="source">The source URL of the file to be copied.</param>
    /// <param name="target">The target directory or file path to copy the file to.</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    TBuilderEntity WithResourceMapping(Uri source, string target, UnixFileModes fileMode = Unix.FileMode644);

    /// <summary>
    /// Assigns the mount configuration to manage data in the container.
    /// </summary>
    /// <param name="mount">The mount configuration.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithMount(IMount mount);

    /// <summary>
    /// Binds and mounts the specified host machine volume into the container.
    /// </summary>
    /// <param name="source">An absolute path or a name value within the host machine.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithBindMount(string source, string destination);

    /// <summary>
    /// Binds and mounts the specified host machine volume into the container.
    /// </summary>
    /// <param name="source">An absolute path or a name value within the host machine.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <param name="accessMode">The volume access mode.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithBindMount(string source, string destination, AccessMode accessMode);

    /// <summary>
    /// Mounts the specified managed volume into the container.
    /// </summary>
    /// <param name="source">The name of the managed volume.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithVolumeMount(string source, string destination);

    /// <summary>
    /// Mounts the specified managed volume into the container.
    /// </summary>
    /// <param name="source">The name of the managed volume.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <param name="accessMode">The volume access mode.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithVolumeMount(string source, string destination, AccessMode accessMode);

    /// <summary>
    /// Mounts the specified managed volume into the container.
    /// </summary>
    /// <param name="volume">The managed volume.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithVolumeMount(IVolume volume, string destination);

    /// <summary>
    /// Mounts the specified managed volume into the container.
    /// </summary>
    /// <param name="volume">The managed volume.</param>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <param name="accessMode">The volume access mode.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithVolumeMount(IVolume volume, string destination, AccessMode accessMode);

    /// <summary>
    /// Mounts the specified tmpfs (temporary file system) volume into the container.
    /// </summary>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithTmpfsMount(string destination);

    /// <summary>
    /// Mounts the specified tmpfs (temporary file system) volume into the container.
    /// </summary>
    /// <param name="destination">An absolute path as destination in the container.</param>
    /// <param name="accessMode">The volume access mode.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithTmpfsMount(string destination, AccessMode accessMode);

    /// <summary>
    /// Assigns the specified network to the container.
    /// </summary>
    /// <param name="name">The network's name to connect to.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithNetwork(string name);

    /// <summary>
    /// Assigns the specified network to the container.
    /// </summary>
    /// <param name="network">The network to connect container to.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithNetwork(INetwork network);

    /// <summary>
    /// Assigns the specified network-scoped aliases to the container.
    /// </summary>
    /// <param name="networkAliases">The network-scoped aliases.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithNetworkAliases(params string[] networkAliases);

    /// <summary>
    /// Assigns the specified network-scoped aliases to the container.
    /// </summary>
    /// <param name="networkAliases">A list of network-scoped aliases.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithNetworkAliases(IEnumerable<string> networkAliases);

    /// <summary>
    /// Adds the extra host to "/etc/hosts" respectively "%WINDIR%\\system32\\drivers\\etc\\hosts".
    /// </summary>
    /// <param name="hostname">The hostname.</param>
    /// <param name="ipAddress">The IP address.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithExtraHost(string hostname, string ipAddress);

    /// <summary>
    /// Cleans up the container after it exits.
    /// </summary>
    /// <remarks>
    /// It is recommended to rely on the Resource Reaper to clean up resources: https://dotnet.testcontainers.org/api/resource-reaper/.
    /// </remarks>
    /// <param name="autoRemove">Determines whether Docker removes the container after it exits or not.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithAutoRemove(bool autoRemove);

    /// <summary>
    /// Sets the privileged flag.
    /// </summary>
    /// <param name="privileged">Determines whether the privileged flag is set or not.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithPrivileged(bool privileged);

    /// <summary>
    /// Sets the output consumer to capture the container's stdout and stderr messages.
    /// </summary>
    /// <param name="outputConsumer">The output consumer.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithOutputConsumer(IOutputConsumer outputConsumer);

    /// <summary>
    /// Sets the wait strategies to indicate readiness of the container.
    /// </summary>
    /// <param name="waitStrategy">The wait strategy.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithWaitStrategy(IWaitForContainerOS waitStrategy);

    /// <summary>
    /// Sets a startup callback to invoke after the container start.
    /// </summary>
    /// <remarks>
    /// The callback method is invoked after the container start, but before the wait strategies.
    /// </remarks>
    /// <param name="startupCallback">The callback method to invoke.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    [PublicAPI]
    TBuilderEntity WithStartupCallback(Func<IContainer, CancellationToken, Task> startupCallback);
  }
}
