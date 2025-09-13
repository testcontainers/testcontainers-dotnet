using System;
using System.Threading.Tasks;
using Testcontainers.PostgreSql.Examples;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Testcontainers PostgreSQL WithSSLConfig demo ===\n");

        try
        {
            await PostgreSqlSSLConfigExample.RunExample();
            Console.WriteLine("\nDemo completed successfully.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("\nDemo failed: " + ex);
            Environment.ExitCode = -1;
        }
    }
}
