# Builder configurations

Testcontainers for .NET creates every Docker resource through a builder: containers, images, networks, volumes, and Docker Compose projects. All builders, including the ones that [modules](../modules/index.md) provide, share the same design. The rules below explain what happens when you call builder methods and how multiple calls combine.

## Builders are immutable

Every builder method returns a new builder instance that includes the updated configuration. The existing instance remains unchanged. This lets you share a common configuration and derive different resources from it, for example for A/B testing. Remember to use the returned builder, otherwise your change is lost. See [reusing builder configurations](create_docker_container.md#reusing-builder-configurations) for examples.

## How builder methods combine

When you call a builder method more than once, or call a method that a module has already called, the configurations combine as follows:

- A method that sets a single value, such as the image, name, wait strategy, or startup callback, replaces the previously configured value.
- Methods that configure lists and dictionaries, such as environment variables, labels, port bindings, or mounts, append new values to the existing ones. A dictionary entry with an existing key replaces the previous entry.
- You cannot remove existing list and dictionary values. The exceptions are builder methods that accept a `ComposableEnumerable<T>`, which let you decide how new values combine with existing ones, for example to overwrite them. See [composing command arguments](create_docker_container.md#composing-command-arguments).

## Validation happens on build

Builder methods only record the configuration. The builder validates the final configuration when you call `Build()`, and throws an exception if a mandatory setting is missing or settings conflict. This is why you might see a configuration error at `Build()`, even though the `WithX` call itself succeeded.

## Modules are pre-configured

Modules come pre-configured, and that configuration is opinionated. It is tailored to run the service reliably in tests. A module sets up things like the environment variables, the wait strategy, and the startup callback.

A module applies most of its defaults when you create the builder, before any of your builder calls. Some parts of the configuration depend on your input, though. In that case, the module applies them in `Build()`, once your configuration is final.

You can still call the generic builder methods on a module builder, but because single values replace previous ones, doing so can override the module's defaults. For example, calling `WithWaitStrategy` or `WithStartupCallback` replaces the module's implementation, which can leave the container unprovisioned or never ready. Overriding a module's configuration this way is not supported. If you need full control over the configuration, use the generic `ContainerBuilder` instead.
