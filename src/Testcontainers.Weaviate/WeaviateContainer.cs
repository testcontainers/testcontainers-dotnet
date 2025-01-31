// Copyright (c) Microsoft. All rights reserved.

using DotNet.Testcontainers.Containers;

namespace Testcontainers.Weaviate;

public class WeaviateContainer(WeaviateConfiguration configuration) : DockerContainer(configuration);
