global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Data.Common;
global using System.Linq;
global using System.Threading.Tasks;
global using Dapper;
global using DotNet.Testcontainers.Commons;
global using JetBrains.Annotations;
global using Npgsql;
global using StackExchange.Redis;
global using Testcontainers.PostgreSql;
global using Testcontainers.Redis;
global using Xunit;
global using Xunit.Sdk;
#if XUNIT_V3
global using Xunit.v3;
#else
global using Xunit.Abstractions;
#endif