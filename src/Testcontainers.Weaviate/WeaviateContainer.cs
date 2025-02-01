// Copyright (c) Microsoft. All rights reserved.

namespace Testcontainers.Weaviate;

public class WeaviateContainer(WeaviateConfiguration configuration) : DockerContainer(configuration);
