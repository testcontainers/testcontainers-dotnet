#nullable enable
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace DotNet.Testcontainers.Commons
{
    public static class TestOutputHelperExtensions
    {
        public static void WriteJson<T>(this ITestOutputHelper testOutputHelper, T source, JsonSerializerOptions? jsonSerializerOptions = null)                
        {
            jsonSerializerOptions ??= new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter()},
                MaxDepth = 64,
            };
            var json = JsonSerializer.Serialize(source, jsonSerializerOptions);
            testOutputHelper.WriteLine(json);
        }
    }
}