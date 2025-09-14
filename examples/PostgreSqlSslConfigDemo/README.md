# PostgreSQL WithSSLSettings demo (Testcontainers for .NET)

This small console project demonstrates how to run a PostgreSQL container configured for SSL/TLS using Testcontainers' `WithSSLSettings` API and how to connect to it using Npgsql.

What it does:
- Generates a temporary CA, server, and client certificates on-the-fly (for demo purposes only).
- Starts a `postgres:16-alpine` container with server-side SSL enabled via `WithSSLSettings(ca, serverCert, serverKey)`.
- Shows two connection scenarios:
  1) SSL required (server-auth only; trusting the server cert for demo).
  2) Mutual TLS (client certificate authentication) using the generated client cert/key.
- Runs a simple SQL command and prints SSL-related metadata.

How to run
1. Ensure Docker is installed and running.
2. From the repository root, run:

   ```bash
   cd examples/PostgreSqlSslConfigDemo
   dotnet run -c Release
   ```

You should see output indicating the container starts with SSL and that both SSL-only and client-certificate connections succeed. Temporary certificates will be created in a temp folder and deleted automatically after the run.

Notes
- The example links to the shared implementation file `examples/PostgreSqlSSLConfigExample.cs` to avoid duplication.
- This demo targets .NET 8.0 and uses the local `Testcontainers.PostgreSql` project reference from `src/` so you can test changes to `WithSSLSettings` live.
- Do not use `TrustServerCertificate = true` in production; it is included here only for demonstration.
