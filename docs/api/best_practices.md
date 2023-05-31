# Best Practices

At times, configuring and launching disposable Docker resources for testing purposes can be initially complex. However, modules have already minimized the amount of configuration required to a bare minimum, requiring only a single line of code to create reliable test environments. Nonetheless, developers should be aware of and adhere to these best practices to write efficient and effective tests:

1. Avoid assigning static names to Docker resources such as containers, networks, and volumes. Static names may clash with existing resources, causing tests to fail. Instead, use random assigned names. By default, Docker assigns random names.
2. Avoid assigning static port bindings to containers. Instead, use random assigned host ports `_builder.WithPortBinding(ushort, true)` and retrieve the mapped port with `_container.GetMappedPublicPort(ushort)`.
3. Avoid using `localhost` or `127.0.0.1` to connect to the container. Instead, use the `_container.Hostname` property to access the container from the test host.
4. When setting up a container-to-container communication, use network aliases `_builder.WithNetworkAliases(string)` to connect to the container. Access the service running inside the container through its container port.
5. Avoid mounting local host paths to containers. Instead, use `_container.WithResourceMapping(string, string)` or one of its overloaded members to copy dependent files to the container before it starts.
6. In rare cases, it may be necessary to access the underlying Docker API to configure specific properties that are not exposed by Testcontainers' own API. To get access to all Docker API properties required to create a resource, use `_builder.WithCreateParameterModifier(Action<TCreateResourceEntity>)`.
7. Do not disable the Resource Reaper, as it cleans up remaining test resources. Disabling it may clutter up the test environment.
