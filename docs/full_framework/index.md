# .NET Framework

When working with older versions of the .NET Framework (e.g., .NET Framework 4.x), you may encounter issues related to assembly binding conflicts. These conflicts typically occur when your application requires specific versions of assemblies that are different from the versions being loaded at runtime.

To resolve these conflicts and ensure the correct versions of assemblies are used, binding redirects are often necessary. Binding redirects allow you to specify which version of an assembly should be used by the runtime, preventing errors and version mismatches during execution.

Testcontainers for .NET relies on several external dependencies, which may require different versions of assemblies. Legacy applications or projects targeting the full .NET Framework may not automatically resolve these dependencies correctly, and without binding redirects, runtime errors or unexpected behavior may occur.

In executable .NET Framework projects (such as console apps, web apps, etc.), Visual Studio typically handles binding redirects automatically. However, this is not the case for class libraries or test projects.

For **test projects**, binding redirects are **not automatically added** by Visual Studio, which means you may need to manually configure them in the `App.config` file (or enable [`AutoGenerateBindingRedirects`](https://learn.microsoft.com/dotnet/framework/configure-apps/redirect-assembly-versions#rely-on-automatic-binding-redirection)).
