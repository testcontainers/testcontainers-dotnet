global using System;
global using System.Data.Common;
global using System.Diagnostics;
global using System.Runtime.ExceptionServices;
global using System.Threading;
global using System.Threading.Tasks;
global using DotNet.Testcontainers.Builders;
global using DotNet.Testcontainers.Containers;
global using JetBrains.Annotations;
global using Microsoft.Extensions.Logging;
global using Xunit;
global using Xunit.Sdk;
#if XUNIT_V3
global using Xunit.v3;
global using LifetimeTask = System.Threading.Tasks.ValueTask;
#else
global using Xunit.Abstractions;
global using LifetimeTask = System.Threading.Tasks.Task;
#endif