# DocGen

A tool that generates the XML documentation of the default configuration that each module builder applies.

## How to use:

Run the tool from the repository root. It requires a running Docker environment:

```bash
dotnet run --project tools/Testcontainers.DocGen
```

The tool writes a `*Builder.Generated.xml` file next to each module builder and exits with `1` if an `<include>` tag does not match an element. Regenerate the files whenever the default configuration of a module or the images of its tests change.

Reference the generated documentation inside `<remarks>`:

```csharp
/// <include file="PostgreSqlBuilder.Generated.xml" path="doc/member[@name='ctor']/*" />
/// <include file="PostgreSqlBuilder.Generated.xml" path="doc/member[@name='WithDatabase(System.String)']/*" />
```

The tool describes private wait strategies and startup callbacks with the `<summary>` of their declaration. Add one when you add them.
